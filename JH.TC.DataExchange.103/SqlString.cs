using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JH.TC.DataExchange._103
{
    class SqlString
    {
        //public static string date { get; set; }

        public static string Query(string date)
        {
            string sql = string.Format(@"
WITH target_datetime AS(
	SELECT
		'{0}'::TIMESTAMP AS end_date
) ,target_student AS(
	SELECT 
		student.id
	FROM
		student
		LEFT OUTER JOIN class
			ON class.id = student.ref_class_id
	WHERE
		student.status = 1
		AND class.grade_year IN (3,9)
) ,target_sems_history AS(
	SELECT 
		history.id
		, ('0'||unnest(xpath('/History/@SchoolYear', history_xml))::text)::integer AS school_year
		, ('0'||unnest(xpath('/History/@Semester', history_xml))::text)::integer AS semester
		, ('0'||unnest(xpath('/History/@GradeYear', history_xml))::text)::integer AS grade_year
		, unnest(xpath('/History/@ClassName', history_xml))::text AS class_name
		, unnest(xpath('/History/@DeptName', history_xml))::text AS dept_name
		, unnest(xpath('/History/@SeatNo', history_xml))::text AS seat_no
		, unnest(xpath('/History/@Teacher', history_xml))::text AS teacher
		, unnest(xpath('/History/@SchoolDayCount', history_xml))::text AS school_day_count
		, history_xml
	FROM (
		SELECT 
			id
			, unnest(xpath('/root/History'
			, xmlparse(content '<root>'||sems_history||'</root>'))) as history_xml
		FROM 
			student
		WHERE 
			student.id IN (SELECT id FROM target_student)
	) as history
) ,target_sems_score AS(
	SELECT
		sss.ref_student_id 
		,CASE WHEN AVG(cast( regexp_replace( xpath_string('<root>'||sss.score_info||'</root>','/root/Domains/Domain[@領域=''健康與體育'']/@成績'), '^$', '0') AS float)) >= 60 THEN 1 ELSE 0 END AS 健康與體育
		,CASE WHEN AVG(cast( regexp_replace( xpath_string('<root>'||sss.score_info||'</root>','/root/Domains/Domain[@領域=''藝術與人文'']/@成績'), '^$', '0') AS float)) >= 60 THEN 1 ELSE 0 END AS 藝術與人文
		,CASE WHEN AVG(cast( regexp_replace( xpath_string('<root>'||sss.score_info||'</root>','/root/Domains/Domain[@領域=''綜合活動'']/@成績'), '^$', '0') AS float)) >= 60 THEN 1 ELSE 0 END AS 綜合活動	
	FROM
		target_student
		LEFT OUTER JOIN (
			SELECT
				id
				, CASE WHEN (target_sems_history.grade_year IN(7,1) AND target_sems_history.semester = 1) THEN target_sems_history.school_year ELSE 0 END schoolyear1
				, CASE WHEN (target_sems_history.grade_year IN(7,1) AND target_sems_history.semester = 2) THEN target_sems_history.school_year ELSE 0 END schoolyear2
				, CASE WHEN (target_sems_history.grade_year IN(8,2) AND target_sems_history.semester = 1) THEN target_sems_history.school_year ELSE 0 END schoolyear3
				, CASE WHEN (target_sems_history.grade_year IN(8,2) AND target_sems_history.semester = 2) THEN target_sems_history.school_year ELSE 0 END schoolyear4
				, CASE WHEN (target_sems_history.grade_year IN(9,3) AND target_sems_history.semester = 1) THEN target_sems_history.school_year ELSE 0 END schoolyear5
				, CASE WHEN (target_sems_history.grade_year IN(9,3) AND target_sems_history.semester = 2) THEN target_sems_history.school_year ELSE 0 END schoolyear6
			FROM
				target_sems_history
		) shistory ON target_student.id = shistory.id
		LEFT OUTER JOIN sems_subj_score AS sss ON target_student.id = sss.ref_student_id 
        AND (
	        (sss.school_year = shistory.schoolyear1 AND sss.semester = 1)
	        OR (sss.school_year = shistory.schoolyear2 AND sss.semester = 2)
	        OR (sss.school_year = shistory.schoolyear3 AND sss.semester = 1)
	        OR (sss.school_year = shistory.schoolyear4 AND sss.semester = 2)
	        OR (sss.school_year = shistory.schoolyear5 AND sss.semester = 1)
        )
	WHERE
		sss.ref_student_id IN(SELECT * FROM target_student)
	GROUP BY
		sss.ref_student_id
) ,target_sems_demerit AS(
	SELECT
		target_student.id
		,CASE 
			WHEN SUM(大過) IS NULL 
			THEN 0 
			ELSE SUM(大過) 
		END AS 大過支數
		,CASE 
			WHEN SUM(小過) IS NULL 
			THEN 0 
			ELSE SUM(小過) 
		END AS 小過支數
		,CASE 
			WHEN SUM(警告) IS NULL 
			THEN 0 
			ELSE SUM(警告) 
		END AS 警告支數
	FROM
		target_student
		LEFT OUTER JOIN (
			SELECT
				sems_moral_score.ref_student_id
				, CAST( regexp_replace( xpath_string(sems_moral_score.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@A'), '^$', '0') AS INTEGER) AS 大過
		        , CAST( regexp_replace( xpath_string(sems_moral_score.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@B'), '^$', '0') AS INTEGER) AS 小過
		        , CAST( regexp_replace( xpath_string(sems_moral_score.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@C'), '^$', '0') AS INTEGER) AS 警告
			FROM
				sems_moral_score
		) AS sems_demerit ON target_student.id = sems_demerit.ref_student_id
	GROUP BY 
		target_student.id
) ,target_demerit AS(
	SELECT
		target_student.id
		,CASE 
			WHEN SUM(大過) IS NULL 
			THEN 0 
			ELSE SUM(大過) 
		END AS 大過支數
		,CASE 
			WHEN SUM(小過) IS NULL 
			THEN 0 
			ELSE SUM(小過) 
		END AS 小過支數
		,CASE 
			WHEN SUM(警告) IS NULL 
			THEN 0 
			ELSE SUM(警告) 
		END AS 警告支數
	FROM
		target_student
		LEFT OUTER JOIN(
			SELECT
				discipline.ref_student_id
				, CAST( regexp_replace( xpath_string(discipline.detail,'/Discipline/Demerit/@A'), '^$', '0') AS INTEGER) AS 大過
		        , CAST( regexp_replace( xpath_string(discipline.detail,'/Discipline/Demerit/@B'), '^$', '0') AS INTEGER) AS 小過
		        , CAST( regexp_replace( xpath_string(discipline.detail,'/Discipline/Demerit/@C'), '^$', '0') AS INTEGER) AS 警告
			FROM
				target_datetime
				LEFT OUTER JOIN discipline
					ON discipline.occur_date <= target_datetime.end_date 
			WHERE
				merit_flag = 0
				AND xpath_string(discipline.detail,'/Discipline/Demerit/@Cleared') <> '是'
				AND ref_student_id IN(SELECT * FROM target_student)		
			UNION ALL
			SELECT
				discipline.ref_student_id
				, CAST( regexp_replace( xpath_string(discipline.detail,'/Discipline/Demerit/@A'), '^$', '0') AS INTEGER) AS 大過
		        , CAST( regexp_replace( xpath_string(discipline.detail,'/Discipline/Demerit/@B'), '^$', '0') AS INTEGER) AS 小過
		        , CAST( regexp_replace( xpath_string(discipline.detail,'/Discipline/Demerit/@C'), '^$', '0') AS INTEGER) AS 警告
			FROM
				target_datetime
				LEFT OUTER JOIN discipline
					ON discipline.occur_date <= target_datetime.end_date 
			WHERE
				merit_flag = 0
				AND xpath_string(discipline.detail,'/Discipline/Demerit/@Cleared') = '是'
				AND xpath_string(discipline.detail,'/Discipline/Demerit/@ClearDate')::TIMESTAMP > (SELECT end_date FROM target_datetime)
				AND ref_student_id IN(SELECT id FROM target_student)		
		) AS target_discipline ON target_student.id = target_discipline.ref_student_id
	GROUP BY target_student.id
) ,total_demerit AS(
	SELECT
		total.id
		, CASE WHEN SUM(大過支數) IS NULL THEN 0 ELSE SUM(大過支數) END AS 大過支數
		, CASE WHEN SUM(小過支數) IS NULL THEN 0 ELSE SUM(小過支數) END AS 小過支數
		, CASE WHEN SUM(警告支數) IS NULL THEN 0 ELSE SUM(警告支數) END AS 警告支數
	FROM(
		SELECT * FROM target_demerit
		UNION ALL
		SELECT * FROM target_sems_demerit	
		) AS total
	GROUP BY
		total.id
) ,target_sems_merit AS(
	SELECT
		target_student.id
		,CASE 
			WHEN SUM(大功) IS NULL 
			THEN 0 
			ELSE SUM(大功) 
		END AS 大功支數
		,CASE 
			WHEN SUM(小功) IS NULL 
			THEN 0 
			ELSE SUM(小功) 
		END AS 小功支數
		,CASE 
			WHEN SUM(嘉獎) IS NULL 
			THEN 0 
			ELSE SUM(嘉獎) 
		END AS 嘉獎支數
	FROM
		target_student
		LEFT OUTER JOIN (
			SELECT
				sems_moral_score.ref_student_id
				, CAST( regexp_replace( xpath_string(sems_moral_score.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@A'), '^$', '0') AS INTEGER) AS 大功
		        , CAST( regexp_replace( xpath_string(sems_moral_score.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@B'), '^$', '0') AS INTEGER) AS 小功
		        , CAST( regexp_replace( xpath_string(sems_moral_score.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@C'), '^$', '0') AS INTEGER) AS 嘉獎
			FROM
				sems_moral_score

		) AS sems_merit ON target_student.id = sems_merit.ref_student_id
	GROUP BY 
		target_student.id
) ,target_merit AS(
	SELECT
		ref_student_id AS id
		,CASE 
			WHEN SUM(大功) IS NULL 
			THEN 0 
			ELSE SUM(大功) 
		END AS 大功支數
		,CASE 
			WHEN SUM(小功) IS NULL 
			THEN 0 
			ELSE SUM(小功) 
		END AS 小功支數
		,CASE 
			WHEN SUM(嘉獎) IS NULL 
			THEN 0 
			ELSE SUM(嘉獎) 
		END AS 嘉獎支數
	FROM(
		SELECT
			discipline.ref_student_id
			, CAST( regexp_replace( xpath_string(discipline.detail,'/Discipline/Merit/@A'), '^$', '0') AS INTEGER) AS 大功
	        , CAST( regexp_replace( xpath_string(discipline.detail,'/Discipline/Merit/@B'), '^$', '0') AS INTEGER) AS 小功
	        , CAST( regexp_replace( xpath_string(discipline.detail,'/Discipline/Merit/@C'), '^$', '0') AS INTEGER) AS 嘉獎
		FROM
			target_datetime
			LEFT OUTER JOIN discipline
				ON discipline.occur_date <= target_datetime.end_date 
		WHERE
			merit_flag = 1
			AND ref_student_id IN(SELECT * FROM target_student)		
		) AS target_discipline
	GROUP BY ref_student_id
) ,total_merit AS (
	SELECT
		total.id
		, CASE WHEN SUM(大功支數) IS NULL THEN 0 ELSE SUM(大功支數) END AS 大功支數
		, CASE WHEN SUM(小功支數) IS NULL THEN 0 ELSE SUM(小功支數) END AS 小功支數
		, CASE WHEN SUM(嘉獎支數) IS NULL THEN 0 ELSE SUM(嘉獎支數) END AS 嘉獎支數
	FROM(
		SELECT * FROM target_merit
		UNION ALL
		SELECT * FROM target_sems_merit	
		) AS total
	GROUP BY
		total.id
) ,target_service_learning AS(
	SELECT 
		target_student.id AS ref_student_id
		,SUM(CASE WHEN (x1.school_year = target_sems_history.school_year AND target_sems_history.grade_year IN(7,1) AND x1.semester = 1) THEN x1.hours ELSE 0 END) AS 服務學習時數_七上
		,SUM(CASE WHEN (x1.school_year = target_sems_history.school_year AND target_sems_history.grade_year IN(7,1) AND x1.semester = 2) THEN x1.hours ELSE 0 END) AS 服務學習時數_七下
		,SUM(CASE WHEN (x1.school_year = target_sems_history.school_year AND target_sems_history.grade_year IN(8,2) AND x1.semester = 1) THEN x1.hours ELSE 0 END) AS 服務學習時數_八上
		,SUM(CASE WHEN (x1.school_year = target_sems_history.school_year AND target_sems_history.grade_year IN(8,2) AND x1.semester = 2) THEN x1.hours ELSE 0 END) AS 服務學習時數_八下
		,SUM(CASE WHEN (x1.school_year = target_sems_history.school_year AND target_sems_history.grade_year IN(9,3) AND x1.semester = 1) THEN x1.hours ELSE 0 END) AS 服務學習時數_九上
		,SUM(CASE WHEN (x1.school_year = target_sems_history.school_year AND target_sems_history.grade_year IN(9,3) AND x1.semester = 2) THEN x1.hours ELSE 0 END) AS 服務學習時數_九下
	FROM
		target_student
		LEFT OUTER JOIN target_sems_history
			ON target_sems_history.id::BIGINT = target_student.id 
	    LEFT JOIN $k12.service.learning.record AS x1 
	    	ON (''||target_student.id) = x1.ref_student_id 
	        AND x1.occur_date <= (SELECT * FROM target_datetime)
    GROUP BY target_student.id
) ,target_club AS(
SELECT 
    target_student.id AS ref_student_id
    ,count(DISTINCT ''||x1.school_year||'XX'||x1.semester) AS TIMES
FROM 
    target_student
    LEFT JOIN (
		SELECT 
			target_sems_history.id
			, CASE WHEN (target_sems_history.grade_year IN(7,1) AND target_sems_history.semester = 1) THEN target_sems_history.school_year ELSE 0 END schoolyear1
			, CASE WHEN (target_sems_history.grade_year IN(7,1) AND target_sems_history.semester = 2) THEN target_sems_history.school_year ELSE 0 END schoolyear2
			, CASE WHEN (target_sems_history.grade_year IN(8,2) AND target_sems_history.semester = 1) THEN target_sems_history.school_year ELSE 0 END schoolyear3
			, CASE WHEN (target_sems_history.grade_year IN(8,2) AND target_sems_history.semester = 2) THEN target_sems_history.school_year ELSE 0 END schoolyear4
			, CASE WHEN (target_sems_history.grade_year IN(9,3) AND target_sems_history.semester = 1) THEN target_sems_history.school_year ELSE 0 END schoolyear5
			, CASE WHEN (target_sems_history.grade_year IN(9,3) AND target_sems_history.semester = 2) THEN target_sems_history.school_year ELSE 0 END schoolyear6
            
        FROM 
        	target_sems_history
    )shistory ON target_student.id = shistory.id
    LEFT OUTER JOIN $k12.resultscore.universal AS x1 ON (''||target_student.id) = x1.ref_student_id 
        AND (
	        (x1.school_year = shistory.schoolyear1 AND x1.semester = 1)
	        OR (x1.school_year = shistory.schoolyear2 AND x1.semester = 2)
	        OR (x1.school_year = shistory.schoolyear3 AND x1.semester = 1)
	        OR (x1.school_year = shistory.schoolyear4 AND x1.semester = 2)
	        OR (x1.school_year = shistory.schoolyear5 AND x1.semester = 1)
	        OR (x1.school_year = shistory.schoolyear6 AND x1.semester = 2)
        )
GROUP BY target_student.id
)

SELECT 
	target_student.id
	,CASE WHEN target_sems_score.健康與體育 is null THEN 0 ELSE target_sems_score.健康與體育 END as 健康與體育
    ,CASE WHEN target_sems_score.藝術與人文 is null THEN 0 ELSE target_sems_score.藝術與人文 END as 藝術與人文
    ,CASE WHEN target_sems_score.綜合活動 is null THEN 0 ELSE target_sems_score.綜合活動 END as 綜合活動 
    ,CASE 
        WHEN (total_demerit.大過支數 * 9 + total_demerit.小過支數 * 3 + total_demerit.警告支數) = 0 THEN 6 
        WHEN (total_demerit.大過支數 * 9 + total_demerit.小過支數 * 3 + total_demerit.警告支數) >= 3 THEN 0 
        ELSE 3 
    END as 記過紀錄
    ,CASE WHEN total_merit.大功支數 is null THEN 0 ELSE total_merit.大功支數 END as 大功支數
    ,CASE WHEN total_merit.小功支數 is null THEN 0 ELSE total_merit.小功支數 END as 小功支數
    ,CASE WHEN total_merit.嘉獎支數 is null THEN 0 ELSE total_merit.嘉獎支數 END as 嘉獎支數
    ,LEAST(
        3 , 
        (
            CASE WHEN target_service_learning.服務學習時數_七上 >= 6 THEN 1 ELSE 0 END +
            CASE WHEN target_service_learning.服務學習時數_七下 >= 6 THEN 1 ELSE 0 END +
            CASE WHEN target_service_learning.服務學習時數_八上 >= 6 THEN 1 ELSE 0 END +
            CASE WHEN target_service_learning.服務學習時數_八下 >= 6 THEN 1 ELSE 0 END +
            CASE WHEN target_service_learning.服務學習時數_九上 >= 6 THEN 1 ELSE 0 END +
            CASE WHEN target_service_learning.服務學習時數_九下 >= 6 THEN 1 ELSE 0 END
        )
    ) AS 服務學習得分
    ,LEAST(
        2,
        CASE WHEN target_club.times IS NULL THEN 0 ELSE target_club.times END
    ) AS 社團得分
FROM 
	target_student
	LEFT OUTER JOIN target_sems_score
		ON target_sems_score.ref_student_id = target_student.id
	LEFT OUTER JOIN total_demerit
		ON total_demerit.id = target_student.id
	LEFT OUTER JOIN total_merit
		ON total_merit.id = target_student.id
	LEFT OUTER JOIN target_service_learning
		ON target_service_learning.ref_student_id = target_student.id
	LEFT OUTER JOIN target_club
		ON target_club.ref_student_id = target_student.id
            ", date);

            return sql;
        }

        #region 舊SQL
//        public static string Query1 = @"
//select 
//    student.id
//    ,CASE WHEN sss.健康與體育 is null THEN 0 ELSE sss.健康與體育 END as ""健康與體育""
//    ,CASE WHEN sss.藝術與人文 is null THEN 0 ELSE sss.藝術與人文 END as ""藝術與人文""
//    ,CASE WHEN sss.綜合活動 is null THEN 0 ELSE sss.綜合活動 END as ""綜合活動""
//    ,CASE 
//        WHEN (discde.大過支數 * 9 + discde.小過支數 * 3 + discde.警告支數) = 0 THEN 6 
//        WHEN (discde.大過支數 * 9 + discde.小過支數 * 3 + discde.警告支數) >= 3 THEN 0 
//        ELSE 3 
//    END as ""記過紀錄""
//    ,CASE WHEN disc.大功支數 is null THEN 0 ELSE disc.大功支數 END as ""大功支數""
//    ,CASE WHEN disc.小功支數 is null THEN 0 ELSE disc.小功支數 END as ""小功支數""
//    ,CASE WHEN disc.嘉獎支數 is null THEN 0 ELSE disc.嘉獎支數 END as ""嘉獎支數""
//    ,LEAST(
//        3 , 
//        (
//            CASE WHEN slr.服務學習時數_七上 >= 6 THEN 1 ELSE 0 END +
//            CASE WHEN slr.服務學習時數_七下 >= 6 THEN 1 ELSE 0 END +
//            CASE WHEN slr.服務學習時數_八上 >= 6 THEN 1 ELSE 0 END +
//            CASE WHEN slr.服務學習時數_八下 >= 6 THEN 1 ELSE 0 END +
//            CASE WHEN slr.服務學習時數_九上 >= 6 THEN 1 ELSE 0 END +
//            CASE WHEN slr.服務學習時數_九下 >= 6 THEN 1 ELSE 0 END
//        )
//    ) as ""服務學習得分""
//    ,LEAST(
//        2,
//        CASE WHEN rs.times is null THEN 0 ELSE rs.times END
//    ) as ""社團得分""
//from 
//    student 
//    left outer join class on student.ref_class_id=class.id
//    left outer join 
//    (
//        select student.id
//			,case when avg(cast( regexp_replace( xpath_string('<root>'||x1.score_info||'</root>','/root/Domains/Domain[@領域=''健康與體育'']/@成績'), '^$', '0') as float)) >= 60 then 1 else 0 end as ""健康與體育""
//			,case when avg(cast( regexp_replace( xpath_string('<root>'||x1.score_info||'</root>','/root/Domains/Domain[@領域=''藝術與人文'']/@成績'), '^$', '0') as float)) >= 60 then 1 else 0 end as ""藝術與人文""
//			,case when avg(cast( regexp_replace( xpath_string('<root>'||x1.score_info||'</root>','/root/Domains/Domain[@領域=''綜合活動'']/@成績'), '^$', '0') as float)) >= 60 then 1 else 0 end as ""綜合活動""
//		from 
//			student join class on student.ref_class_id=class.id and class.grade_year in (3, 9)
//			left join 
//            (
//				SELECT student.id
//	                , ''||g1.SchoolYear as schoolyear1
//	                , ''||g2.SchoolYear as schoolyear2
//	                , ''||g3.SchoolYear as schoolyear3
//	                , ''||g4.SchoolYear as schoolyear4
//	                , ''||g5.SchoolYear as schoolyear5
//	                , ''||g6.SchoolYear as schoolyear6
//                FROM 
//                    student 
//                    left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g1 on g1.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g2 on g2.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g3 on g3.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g4 on g4.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g5 on g5.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g6 on g6.id=student.id LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9)
//            )shistory on student.id=shistory.id
//			left join sems_subj_score as x1 on student.id=x1.ref_student_id
//			    and (
//				    (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
//                    or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
//				    or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
//				    or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
//				    or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
//			    )
//	    group by student.id
//    ) as sss on student.id = sss.id
//    left outer join 
//    (
//	    select 
//			student.id
//			,CASE WHEN sum(大功) is null THEN 0 ELSE sum(大功) END as ""大功支數""
//			,CASE WHEN sum(小功) is null THEN 0 ELSE sum(小功) END as ""小功支數""
//			,CASE WHEN sum(嘉獎) is null THEN 0 ELSE sum(嘉獎) END as ""嘉獎支數""
//		from 
//			student join class on student.ref_class_id=class.id and class.grade_year in (3, 9)
//			left join (
//				SELECT student.id
//		            , ''||g1.SchoolYear as schoolyear1
//		            , ''||g2.SchoolYear as schoolyear2
//		            , ''||g3.SchoolYear as schoolyear3
//		            , ''||g4.SchoolYear as schoolyear4
//		            , ''||g5.SchoolYear as schoolyear5
//		            , ''||g6.SchoolYear as schoolyear6
//	            FROM student 
//	            left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g1 on g1.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g2 on g2.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g3 on g3.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g4 on g4.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g5 on g5.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g6 on g6.id=student.id LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9)
//	        )shistory on student.id=shistory.id
//			left join (
//		        select 	
//			        ref_student_id
//			        ,school_year
//			        ,semester
//			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@A'), '^$', '0') as integer) as ""大功""
//			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@B'), '^$', '0') as integer) as ""小功""
//			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@C'), '^$', '0') as integer) as ""嘉獎""
//		        from 
//			        student
//			        left outer join class on student.ref_class_id=class.id
//			        left outer join discipline as x1 on student.id=x1.ref_student_id
//		        where 
//			        student.status = 1 
//			        and class.grade_year in (3, 9)
//			        and merit_flag = 1

//                UNION ALL

//		        select 
//			        ref_student_id
//			        ,school_year
//			        ,semester
//			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@A'), '^$', '0') as integer) as ""大功""
//			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@B'), '^$', '0') as integer) as ""小功""
//			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@C'), '^$', '0') as integer) as ""嘉獎""
//		        from 
//			        student
//			        left outer join class on student.ref_class_id=class.id
//			        left outer join sems_moral_score as x1 on student.id=x1.ref_student_id
//		        where 
//			        student.status = 1 
//			        and class.grade_year in (3, 9)
//			        and ( 
//				        CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@A'), '^$', '0') as integer) > 0
//				        or CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@B'), '^$', '0')  as integer) > 0
//				        or CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@C'), '^$', '0')  as integer) > 0
//			        )
//			) as x1 on student.id=x1.ref_student_id
//			    and (
//				    (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
//				    or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
//				    or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
//				    or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
//				    or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
//			    )
//		group by student.id
//    )as disc on student.id = disc.id
//left outer join 
//    (
//	    select 
//			student.id
//			,CASE WHEN sum(大過) is null THEN 0 ELSE sum(大過) END as ""大過支數""
//			,CASE WHEN sum(小過) is null THEN 0 ELSE sum(小過) END as ""小過支數""
//			,CASE WHEN sum(警告) is null THEN 0 ELSE sum(警告) END as ""警告支數""
//		from 
//			student join class on student.ref_class_id=class.id and class.grade_year in (3, 9)
//			left join (
//				SELECT student.id
//		            , ''||g1.SchoolYear as schoolyear1
//		            , ''||g2.SchoolYear as schoolyear2
//		            , ''||g3.SchoolYear as schoolyear3
//		            , ''||g4.SchoolYear as schoolyear4
//		            , ''||g5.SchoolYear as schoolyear5
//		            , ''||g6.SchoolYear as schoolyear6
//	            FROM student 
//	                left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g1 on g1.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g2 on g2.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g3 on g3.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g4 on g4.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g5 on g5.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g6 on g6.id=student.id LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9)
//	        )shistory on student.id=shistory.id
//			left join (
//		        select 	
//			        ref_student_id
//			        ,school_year
//			        ,semester
//			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@A'), '^$', '0') as integer) as ""大過""
//			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@B'), '^$', '0') as integer) as ""小過""
//			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@C'), '^$', '0') as integer) as ""警告""
//		        from 
//			        student
//			        left outer join class on student.ref_class_id=class.id
//			        left outer join discipline as x1 on student.id=x1.ref_student_id
//		        where 
//			        student.status = 1 
//			        and class.grade_year in (3, 9)
//			        and merit_flag = 0
//			        and xpath_string(x1.detail,'/Discipline/Demerit/@Cleared') <> '是'

//                UNION ALL

//		        select 
//			        ref_student_id
//			        ,school_year
//			        ,semester
//			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@A'), '^$', '0') as integer) as ""大過""
//			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@B'), '^$', '0') as integer) as ""小過""
//			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@C'), '^$', '0') as integer) as ""警告""
//		        from 
//			        student
//			        left outer join class on student.ref_class_id=class.id
//			        left outer join sems_moral_score as x1 on student.id=x1.ref_student_id
//		        where 
//			        student.status = 1 
//			        and class.grade_year in (3, 9)
//			        and ( 
//				        CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@A'), '^$', '0') as integer) > 0
//				        or CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@B'), '^$', '0')  as integer) > 0
//				        or CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@C'), '^$', '0')  as integer) > 0
//			        )
//			) as x1 on student.id=x1.ref_student_id
//			    and (
//                    (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
//                    or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
//				    or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
//				    or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
//				    or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
//			    )
//		group by student.id
//    )as discde on student.id = discde.id
//    left outer join 
//    (
//	    select student.id
//			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1) THEN x1.hours ELSE 0 END) as ""服務學習時數_七上""
//			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2) THEN x1.hours ELSE 0 END) as ""服務學習時數_七下""
//			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1) THEN x1.hours ELSE 0 END) as ""服務學習時數_八上""
//			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2) THEN x1.hours ELSE 0 END) as ""服務學習時數_八下""
//			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1) THEN x1.hours ELSE 0 END) as ""服務學習時數_九上""
//			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2) THEN x1.hours ELSE 0 END) as ""服務學習時數_九下""
//		from 
//			student join class on student.ref_class_id=class.id and class.grade_year in (3, 9)
//			left join (
//				SELECT student.id
//		            , ''||g1.SchoolYear as schoolyear1
//		            , ''||g2.SchoolYear as schoolyear2
//		            , ''||g3.SchoolYear as schoolyear3
//		            , ''||g4.SchoolYear as schoolyear4
//		            , ''||g5.SchoolYear as schoolyear5
//		            , ''||g6.SchoolYear as schoolyear6
//	            FROM student 
//	                left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g1 on g1.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g2 on g2.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g3 on g3.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g4 on g4.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g5 on g5.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g6 on g6.id=student.id LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9)
//	        )shistory on student.id=shistory.id
//		    left join $k12.service.learning.record as x1 on (''||student.id)=x1.ref_student_id 
//		        and (
//			        (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
//			        or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
//			        or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
//			        or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
//			        or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
//			        or (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2)
//		        )
//	    group by student.id
//    ) as slr on student.id = slr.id
//    left outer join
//    (
//        select 
//            student.id as id
//            ,count(DISTINCT ''||x1.school_year||'XX'||x1.semester) as times
//        from 
//            student join class on student.ref_class_id=class.id and class.grade_year in (3, 9)
//            left join (
//				SELECT student.id
//		            , ''||g1.SchoolYear as schoolyear1
//		            , ''||g2.SchoolYear as schoolyear2
//		            , ''||g3.SchoolYear as schoolyear3
//		            , ''||g4.SchoolYear as schoolyear4
//		            , ''||g5.SchoolYear as schoolyear5
//		            , ''||g6.SchoolYear as schoolyear6
//	            FROM student 
//	                left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g1 on g1.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g2 on g2.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g3 on g3.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g4 on g4.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g5 on g5.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g6 on g6.id=student.id LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9)
//	        )shistory on student.id=shistory.id
//            left outer join $k12.resultscore.universal as x1 on (''||student.id)=x1.ref_student_id 
//		        and (
//			        (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
//			        or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
//			        or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
//			        or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
//			        or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
//			        or (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2)
//		        )
//        group by student.id
//    ) as rs on student.id = rs.id
//WHERE 
//    student.status = 1 
//    and class.grade_year in (3, 9)";

        #endregion

    }
}