using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JH.TC.DataExchange._103
{
    class SqlString
    {
        public static string Query1 = @"
select 
    student.id
    ,CASE WHEN sss.健康與體育 is null THEN 0 ELSE sss.健康與體育 END as ""健康與體育""
    ,CASE WHEN sss.藝術與人文 is null THEN 0 ELSE sss.藝術與人文 END as ""藝術與人文""
    ,CASE WHEN sss.綜合活動 is null THEN 0 ELSE sss.綜合活動 END as ""綜合活動""
    ,CASE 
        WHEN (discde.大過支數 * 9 + discde.小過支數 * 3 + discde.警告支數) = 0 THEN 6 
        WHEN (discde.大過支數 * 9 + discde.小過支數 * 3 + discde.警告支數) >= 3 THEN 0 
        ELSE 3 
    END as ""記過紀錄""
    ,CASE WHEN disc.大功支數 is null THEN 0 ELSE disc.大功支數 END as ""大功支數""
    ,CASE WHEN disc.小功支數 is null THEN 0 ELSE disc.小功支數 END as ""小功支數""
    ,CASE WHEN disc.嘉獎支數 is null THEN 0 ELSE disc.嘉獎支數 END as ""嘉獎支數""
    ,LEAST(
        3 , 
        (
            CASE WHEN slr.服務學習時數_七上 >= 6 THEN 1 ELSE 0 END +
            CASE WHEN slr.服務學習時數_七下 >= 6 THEN 1 ELSE 0 END +
            CASE WHEN slr.服務學習時數_八上 >= 6 THEN 1 ELSE 0 END +
            CASE WHEN slr.服務學習時數_八下 >= 6 THEN 1 ELSE 0 END +
            CASE WHEN slr.服務學習時數_九上 >= 6 THEN 1 ELSE 0 END +
            CASE WHEN slr.服務學習時數_九下 >= 6 THEN 1 ELSE 0 END
        )
    ) as ""服務學習得分""
    ,LEAST(
        2,
        CASE WHEN rs.times is null THEN 0 ELSE rs.times END
    ) as ""社團得分""
from 
    student 
    left outer join class on student.ref_class_id=class.id
    left outer join 
    (
        select student.id
			,case when avg(cast( regexp_replace( xpath_string('<root>'||x1.score_info||'</root>','/root/Domains/Domain[@領域=''健康與體育'']/@成績'), '^$', '0') as float)) >= 60 then 4 else 0 end as ""健康與體育""
			,case when avg(cast( regexp_replace( xpath_string('<root>'||x1.score_info||'</root>','/root/Domains/Domain[@領域=''藝術與人文'']/@成績'), '^$', '0') as float)) >= 60 then 4 else 0 end as ""藝術與人文""
			,case when avg(cast( regexp_replace( xpath_string('<root>'||x1.score_info||'</root>','/root/Domains/Domain[@領域=''綜合活動'']/@成績'), '^$', '0') as float)) >= 60 then 4 else 0 end as ""綜合活動""
		from 
			student join class on student.ref_class_id=class.id and class.grade_year in (3, 9)
			left join 
            (
				SELECT student.id
	                , ''||g1.SchoolYear as schoolyear1
	                , ''||g2.SchoolYear as schoolyear2
	                , ''||g3.SchoolYear as schoolyear3
	                , ''||g4.SchoolYear as schoolyear4
	                , ''||g5.SchoolYear as schoolyear5
	                , ''||g6.SchoolYear as schoolyear6
                FROM 
                    student 
                    left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g1 on g1.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g2 on g2.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g3 on g3.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g4 on g4.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g5 on g5.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g6 on g6.id=student.id LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join sems_subj_score as x1 on student.id=x1.ref_student_id
			    and (
				    (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
				    or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
				    or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
			    )
	    group by student.id
    ) as sss on student.id = sss.id
    left outer join 
    (
	    select 
			student.id
			,CASE WHEN sum(大功) is null THEN 0 ELSE sum(大功) END as ""大功支數""
			,CASE WHEN sum(小功) is null THEN 0 ELSE sum(小功) END as ""小功支數""
			,CASE WHEN sum(嘉獎) is null THEN 0 ELSE sum(嘉獎) END as ""嘉獎支數""
		from 
			student join class on student.ref_class_id=class.id and class.grade_year in (3, 9)
			left join (
				SELECT student.id
		            , ''||g1.SchoolYear as schoolyear1
		            , ''||g2.SchoolYear as schoolyear2
		            , ''||g3.SchoolYear as schoolyear3
		            , ''||g4.SchoolYear as schoolyear4
		            , ''||g5.SchoolYear as schoolyear5
		            , ''||g6.SchoolYear as schoolyear6
	            FROM student 
	            left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g1 on g1.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g2 on g2.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g3 on g3.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g4 on g4.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g5 on g5.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g6 on g6.id=student.id LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9)
	        )shistory on student.id=shistory.id
			left join (
		        select 	
			        ref_student_id
			        ,school_year
			        ,semester
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@A'), '^$', '0') as integer) as ""大功""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@B'), '^$', '0') as integer) as ""小功""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@C'), '^$', '0') as integer) as ""嘉獎""
		        from 
			        student
			        left outer join class on student.ref_class_id=class.id
			        left outer join discipline as x1 on student.id=x1.ref_student_id
		        where 
			        student.status = 1 
			        and class.grade_year in (3, 9)
			        and merit_flag = 1

                UNION ALL

		        select 
			        ref_student_id
			        ,school_year
			        ,semester
			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@A'), '^$', '0') as integer) as ""大功""
			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@B'), '^$', '0') as integer) as ""小功""
			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@C'), '^$', '0') as integer) as ""嘉獎""
		        from 
			        student
			        left outer join class on student.ref_class_id=class.id
			        left outer join sems_moral_score as x1 on student.id=x1.ref_student_id
		        where 
			        student.status = 1 
			        and class.grade_year in (3, 9)
			        and ( 
				        CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@A'), '^$', '0') as integer) > 0
				        or CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@B'), '^$', '0')  as integer) > 0
				        or CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Merit/@C'), '^$', '0')  as integer) > 0
			        )
			) as x1 on student.id=x1.ref_student_id
			    and (
				    (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
				    or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
				    or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
				    or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
				    or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
			    )
		group by student.id
    )as disc on student.id = disc.id
left outer join 
    (
	    select 
			student.id
			,CASE WHEN sum(大過) is null THEN 0 ELSE sum(大過) END as ""大過支數""
			,CASE WHEN sum(小過) is null THEN 0 ELSE sum(小過) END as ""小過支數""
			,CASE WHEN sum(警告) is null THEN 0 ELSE sum(警告) END as ""警告支數""
		from 
			student join class on student.ref_class_id=class.id and class.grade_year in (3, 9)
			left join (
				SELECT student.id
		            , ''||g1.SchoolYear as schoolyear1
		            , ''||g2.SchoolYear as schoolyear2
		            , ''||g3.SchoolYear as schoolyear3
		            , ''||g4.SchoolYear as schoolyear4
		            , ''||g5.SchoolYear as schoolyear5
		            , ''||g6.SchoolYear as schoolyear6
	            FROM student 
	                left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g1 on g1.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g2 on g2.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g3 on g3.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g4 on g4.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g5 on g5.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g6 on g6.id=student.id LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9)
	        )shistory on student.id=shistory.id
			left join (
		        select 	
			        ref_student_id
			        ,school_year
			        ,semester
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@A'), '^$', '0') as integer) as ""大過""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@B'), '^$', '0') as integer) as ""小過""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@C'), '^$', '0') as integer) as ""警告""
		        from 
			        student
			        left outer join class on student.ref_class_id=class.id
			        left outer join discipline as x1 on student.id=x1.ref_student_id
		        where 
			        student.status = 1 
			        and class.grade_year in (3, 9)
			        and merit_flag = 0
			        and xpath_string(x1.detail,'/Discipline/Demerit/@Cleared') <> '是'

                UNION ALL

		        select 
			        ref_student_id
			        ,school_year
			        ,semester
			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@A'), '^$', '0') as integer) as ""大過""
			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@B'), '^$', '0') as integer) as ""小過""
			        ,CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@C'), '^$', '0') as integer) as ""警告""
		        from 
			        student
			        left outer join class on student.ref_class_id=class.id
			        left outer join sems_moral_score as x1 on student.id=x1.ref_student_id
		        where 
			        student.status = 1 
			        and class.grade_year in (3, 9)
			        and ( 
				        CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@A'), '^$', '0') as integer) > 0
				        or CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@B'), '^$', '0')  as integer) > 0
				        or CAST( regexp_replace( xpath_string(x1.initial_summary,'/InitialSummary/DisciplineStatistics/Demerit/@C'), '^$', '0')  as integer) > 0
			        )
			) as x1 on student.id=x1.ref_student_id
			    and (
                    (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
				    or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
				    or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
			    )
		group by student.id
    )as discde on student.id = discde.id
    left outer join 
    (
	    select student.id
			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1) THEN x1.hours ELSE 0 END) as ""服務學習時數_七上""
			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2) THEN x1.hours ELSE 0 END) as ""服務學習時數_七下""
			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1) THEN x1.hours ELSE 0 END) as ""服務學習時數_八上""
			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2) THEN x1.hours ELSE 0 END) as ""服務學習時數_八下""
			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1) THEN x1.hours ELSE 0 END) as ""服務學習時數_九上""
			,sum(CASE WHEN (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2) THEN x1.hours ELSE 0 END) as ""服務學習時數_九下""
		from 
			student join class on student.ref_class_id=class.id and class.grade_year in (3, 9)
			left join (
				SELECT student.id
		            , ''||g1.SchoolYear as schoolyear1
		            , ''||g2.SchoolYear as schoolyear2
		            , ''||g3.SchoolYear as schoolyear3
		            , ''||g4.SchoolYear as schoolyear4
		            , ''||g5.SchoolYear as schoolyear5
		            , ''||g6.SchoolYear as schoolyear6
	            FROM student 
	                left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g1 on g1.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g2 on g2.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g3 on g3.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g4 on g4.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g5 on g5.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g6 on g6.id=student.id LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9)
	        )shistory on student.id=shistory.id
		    left join $k12.service.learning.record as x1 on (''||student.id)=x1.ref_student_id 
		        and (
			        (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
			        or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
			        or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
			        or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
			        or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
			        or (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2)
		        )
	    group by student.id
    ) as slr on student.id = slr.id
    left outer join
    (
        select 
            student.id as id
            ,count(DISTINCT ''||x1.school_year||'XX'||x1.semester) as times
        from 
            student join class on student.ref_class_id=class.id and class.grade_year in (3, 9)
            left join (
				SELECT student.id
		            , ''||g1.SchoolYear as schoolyear1
		            , ''||g2.SchoolYear as schoolyear2
		            , ''||g3.SchoolYear as schoolyear3
		            , ''||g4.SchoolYear as schoolyear4
		            , ''||g5.SchoolYear as schoolyear5
		            , ''||g6.SchoolYear as schoolyear6
	            FROM student 
	                left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g1 on g1.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g2 on g2.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g3 on g3.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g4 on g4.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g5 on g5.id=student.id left outer join (SELECT id, max(SchoolYear) as SchoolYear FROM xpath_table( 'id', '''<root>''||sems_history||''</root>''', 'student', '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear', 'id IN ( select student.id from student LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9) )') AS tmp(id int, SchoolYear integer) group by id )as g6 on g6.id=student.id LEFT OUTER JOIN class ON student.ref_class_id = class.id WHERE student.status=1 AND class.grade_year in (3, 9)
	        )shistory on student.id=shistory.id
            left outer join $k12.resultscore.universal as x1 on (''||student.id)=x1.ref_student_id 
		        and (
			        (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
			        or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
			        or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
			        or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
			        or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
			        or (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2)
		        )
        group by student.id
    ) as rs on student.id = rs.id
WHERE 
    student.status = 1 
    and class.grade_year in (3, 9)";
    }
}