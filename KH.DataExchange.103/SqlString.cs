using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KH.DataExchange._103
{
    class SqlString
    {
        #region 多元成績MultivariateScore
        public static string MultivariateScore =
@"select 	(
		select 
			xpath_string(content,'/SchoolInformation/Code') 
		from list 
		where name = '學校資訊'
	)as ""國中學校代碼""
	, substr(class.class_name, length(class.class_name)-1, 2) as ""班級""
	, student.id_number as ""身分證字號""
    ,'' as ""國一入學年度""
	,'1' as ""是否就讀滿一學年""
    ,'' as ""成績未滿6學期""
	,s1.""科目""
	,s1.""7上成績""
	,s1.""7下成績""
	,s1.""8上成績""
	,s1.""8下成績""
	,s1.""9上成績""
    ,s1.""9下成績""
    ,LPAD(CAST(class.display_order AS VARCHAR),2,'0') as ""學生班級""
    ,student.seat_no as ""學生座號""
    ,student.name as ""學生姓名""
from 
	student
	left outer join class on student.ref_class_id=class.id
	left outer join (
		select student.id
			, '心肺適能' as ""科目""
			,x1.cardiorespiratory_degree as ""7上成績""
			,x2.cardiorespiratory_degree as ""7下成績""
			,x3.cardiorespiratory_degree as ""8上成績""
			,x4.cardiorespiratory_degree as ""8下成績""
			,x5.cardiorespiratory_degree as ""9上成績""
            ,x6.cardiorespiratory_degree as ""9下成績""
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
                left outer join (		
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join $ischool_student_fitness as x1 on (''||student.id)=x1.ref_student_id and (''||x1.school_year)=shistory.schoolyear1
			left join $ischool_student_fitness as x2 on (''||student.id)=x2.ref_student_id and (''||x2.school_year)=shistory.schoolyear2
			left join $ischool_student_fitness as x3 on (''||student.id)=x3.ref_student_id and (''||x3.school_year)=shistory.schoolyear3
			left join $ischool_student_fitness as x4 on (''||student.id)=x4.ref_student_id and (''||x4.school_year)=shistory.schoolyear4
			left join $ischool_student_fitness as x5 on (''||student.id)=x5.ref_student_id and (''||x5.school_year)=shistory.schoolyear5 
            left join $ischool_student_fitness as x6 on (''||student.id)=x6.ref_student_id and (''||x6.school_year)=shistory.schoolyear6 
		--_$_83 =  $ischool_student_fitness
	UNION ALL
		select student.id
			, '仰臥起坐' as ""科目""
			,x1.sit_up_degree as ""7上成績""
			,x2.sit_up_degree as ""7下成績""
			,x3.sit_up_degree as ""8上成績""
			,x4.sit_up_degree as ""8下成績""
			,x5.sit_up_degree as ""9上成績""
            ,x6.sit_up_degree as ""9下成績""
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
                left outer join (     
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join $ischool_student_fitness as x1 on (''||student.id)=x1.ref_student_id and (''||x1.school_year)=shistory.schoolyear1
			left join $ischool_student_fitness as x2 on (''||student.id)=x2.ref_student_id and (''||x2.school_year)=shistory.schoolyear2
			left join $ischool_student_fitness as x3 on (''||student.id)=x3.ref_student_id and (''||x3.school_year)=shistory.schoolyear3
			left join $ischool_student_fitness as x4 on (''||student.id)=x4.ref_student_id and (''||x4.school_year)=shistory.schoolyear4
			left join $ischool_student_fitness as x5 on (''||student.id)=x5.ref_student_id and (''||x5.school_year)=shistory.schoolyear5 
            left join $ischool_student_fitness as x6 on (''||student.id)=x6.ref_student_id and (''||x6.school_year)=shistory.schoolyear6
		--_$_83 =  $ischool_student_fitness
	UNION ALL
		select student.id
			, '立定跳遠' as ""科目""
			,x1.standing_long_jump_degree as ""7上成績""
			,x2.standing_long_jump_degree as ""7下成績""
			,x3.standing_long_jump_degree as ""8上成績""
			,x4.standing_long_jump_degree as ""8下成績""
			,x5.standing_long_jump_degree as ""9上成績""
            ,x6.standing_long_jump_degree as ""9下成績""
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
                left outer join (    
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join $ischool_student_fitness as x1 on (''||student.id)=x1.ref_student_id and (''||x1.school_year)=shistory.schoolyear1
			left join $ischool_student_fitness as x2 on (''||student.id)=x2.ref_student_id and (''||x2.school_year)=shistory.schoolyear2
			left join $ischool_student_fitness as x3 on (''||student.id)=x3.ref_student_id and (''||x3.school_year)=shistory.schoolyear3
			left join $ischool_student_fitness as x4 on (''||student.id)=x4.ref_student_id and (''||x4.school_year)=shistory.schoolyear4
			left join $ischool_student_fitness as x5 on (''||student.id)=x5.ref_student_id and (''||x5.school_year)=shistory.schoolyear5 
            left join $ischool_student_fitness as x6 on (''||student.id)=x6.ref_student_id and (''||x6.school_year)=shistory.schoolyear6 
		--_$_83 =  $ischool_student_fitness
	UNION ALL
		select student.id
			, '坐姿體前彎' as ""科目""
			,x1.sit_and_reach_degree as ""7上成績""
			,x2.sit_and_reach_degree as ""7下成績""
			,x3.sit_and_reach_degree as ""8上成績""
			,x4.sit_and_reach_degree as ""8下成績""
			,x5.sit_and_reach_degree as ""9上成績""
            ,x6.sit_and_reach_degree as ""9下成績""
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
                left outer join (     
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join $ischool_student_fitness as x1 on (''||student.id)=x1.ref_student_id and (''||x1.school_year)=shistory.schoolyear1
			left join $ischool_student_fitness as x2 on (''||student.id)=x2.ref_student_id and (''||x2.school_year)=shistory.schoolyear2
			left join $ischool_student_fitness as x3 on (''||student.id)=x3.ref_student_id and (''||x3.school_year)=shistory.schoolyear3
			left join $ischool_student_fitness as x4 on (''||student.id)=x4.ref_student_id and (''||x4.school_year)=shistory.schoolyear4
			left join $ischool_student_fitness as x5 on (''||student.id)=x5.ref_student_id and (''||x5.school_year)=shistory.schoolyear5 
            left join $ischool_student_fitness as x6 on (''||student.id)=x6.ref_student_id and (''||x6.school_year)=shistory.schoolyear6
		--_$_83 =  $ischool_student_fitness
	UNION ALL
		select student.id
			, '幹部任期次數' as ""科目""
			,''||COUNT(CASE WHEN (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1) THEN CASE WHEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]')  = '幹部' THEN 1 ELSE null END ELSE null END) as ""7上成績""
			,''||COUNT(CASE WHEN (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2) THEN CASE WHEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]')  = '幹部' THEN 1 ELSE null END ELSE null END) as ""7下成績""
			,''||COUNT(CASE WHEN (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1) THEN CASE WHEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]')  = '幹部' THEN 1 ELSE null END ELSE null END) as ""8上成績""
			,''||COUNT(CASE WHEN (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2) THEN CASE WHEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]')  = '幹部' THEN 1 ELSE null END ELSE null END) as ""8下成績""
			,''||COUNT(CASE WHEN (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1) THEN CASE WHEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]')  = '幹部' THEN 1 ELSE null END ELSE null END) as ""9上成績""
            ,''||COUNT(CASE WHEN (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2) THEN CASE WHEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]')  = '幹部' THEN 1 ELSE null END ELSE null END) as ""9下成績""
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
                left outer join (     
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join discipline as x1 on student.id=x1.ref_student_id and x1.merit_flag = 1
			and (
				(''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
				or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
				or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
				or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
				or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
                or (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2)
			)
		where 
			student.status = 1 
			and class.grade_year in (3, 9)
		group by student.id
		--_$_7 =  $behavior.thecadre
	UNION ALL
		select 
			student.id
			,'警告' as ""科目""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1) THEN x1.""警告"" ELSE 0 END) as ""7上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2) THEN x1.""警告"" ELSE 0 END) as ""7下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1) THEN x1.""警告"" ELSE 0 END) as ""8上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2) THEN x1.""警告"" ELSE 0 END) as ""8下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1) THEN x1.""警告"" ELSE 0 END) as ""9上成績""
            ,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2) THEN x1.""警告"" ELSE 0 END) as ""9下成績""
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
                left outer join (     
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join (
		        select 	
			        ref_student_id
			        ,school_year
			        ,semester
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@A'), '^$', '0') as integer) as ""大過""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@B'), '^$', '0') as integer) as ""小過""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@C'), '^$', '0') as integer) as ""警告""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]') END as ""事由類別""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN ltrim( x1.reason,substring(x1.reason from E'^[\[][^\]]*[\]]')) ELSE x1.reason END as ""事由""
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
			        ,'' as ""事由類別""
			        ,school_year || '-' || semester || '獎勵統計' as ""事由""
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
				(''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
				or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
				or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
				or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
				or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
                or (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2)
			)
		where 
			student.status = 1 
			and class.grade_year in (3, 9)
		group by student.id
	UNION ALL
		select 
			student.id
			,'小過' as ""科目""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1) THEN x1.""小過"" ELSE 0 END) as ""7上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2) THEN x1.""小過"" ELSE 0 END) as ""7下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1) THEN x1.""小過"" ELSE 0 END) as ""8上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2) THEN x1.""小過"" ELSE 0 END) as ""8下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1) THEN x1.""小過"" ELSE 0 END) as ""9上成績""
            ,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2) THEN x1.""小過"" ELSE 0 END) as ""9下成績""
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
                left outer join (     	
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join (
		        select 	
			        ref_student_id
			        ,school_year
			        ,semester
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@A'), '^$', '0') as integer) as ""大過""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@B'), '^$', '0') as integer) as ""小過""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@C'), '^$', '0') as integer) as ""警告""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]') END as ""事由類別""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN ltrim( x1.reason,substring(x1.reason from E'^[\[][^\]]*[\]]')) ELSE x1.reason END as ""事由""
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
			        ,'' as ""事由類別""
			        ,'' as ""事由""
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
				(''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
				or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
				or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
				or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
				or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
                or (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2)
			)
		where 
			student.status = 1 
			and class.grade_year in (3, 9)
		group by student.id
	UNION ALL
		select 
			student.id
			,'大過' as ""科目""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1) THEN x1.""大過"" ELSE 0 END) as ""7上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2) THEN x1.""大過"" ELSE 0 END) as ""7下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1) THEN x1.""大過"" ELSE 0 END) as ""8上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2) THEN x1.""大過"" ELSE 0 END) as ""8下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1) THEN x1.""大過"" ELSE 0 END) as ""9上成績""
            ,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2) THEN x1.""大過"" ELSE 0 END) as ""9下成績""
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
                left outer join (     
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join (
		        select 	
			        ref_student_id
			        ,school_year
			        ,semester
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@A'), '^$', '0') as integer) as ""大過""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@B'), '^$', '0') as integer) as ""小過""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Demerit/@C'), '^$', '0') as integer) as ""警告""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]') END as ""事由類別""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN ltrim( x1.reason,substring(x1.reason from E'^[\[][^\]]*[\]]')) ELSE x1.reason END as ""事由""
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
			        ,'' as ""事由類別""
			        ,'' as ""事由""
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
			)as x1 on student.id=x1.ref_student_id
			    and (
				    (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
				    or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
				    or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
				    or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
				    or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
                    or (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2)
			    )
		where 
			student.status = 1 
			and class.grade_year in (3, 9)
		group by student.id
	UNION ALL
		select 
			student.id
			,'嘉獎' as ""科目""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1) THEN x1.""嘉獎"" ELSE 0 END) as ""7上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2) THEN x1.""嘉獎"" ELSE 0 END) as ""7下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1) THEN x1.""嘉獎"" ELSE 0 END) as ""8上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2) THEN x1.""嘉獎"" ELSE 0 END) as ""8下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1) THEN x1.""嘉獎"" ELSE 0 END) as ""9上成績""
            ,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2) THEN x1.""嘉獎"" ELSE 0 END) as ""9下成績""
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
                left outer join (     
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join  (
		        select 	
			        ref_student_id
			        ,school_year
			        ,semester
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@A'), '^$', '0') as integer) as ""大功""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@B'), '^$', '0') as integer) as ""小功""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@C'), '^$', '0') as integer) as ""嘉獎""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]') END as ""事由類別""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN ltrim( x1.reason,substring(x1.reason from E'^[\[][^\]]*[\]]')) ELSE x1.reason END as ""事由""
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
			        ,'' as ""事由類別""
			        ,'' as ""事由""
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
                    or (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2)
			    )
		where 
			student.status = 1 
			and class.grade_year in (3, 9)
		group by student.id
	UNION ALL
		select 
			student.id
			,'小功' as ""科目""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1) THEN x1.""小功"" ELSE 0 END) as ""7上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2) THEN x1.""小功"" ELSE 0 END) as ""7下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1) THEN x1.""小功"" ELSE 0 END) as ""8上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2) THEN x1.""小功"" ELSE 0 END) as ""8下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1) THEN x1.""小功"" ELSE 0 END) as ""9上成績""
            ,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2) THEN x1.""小功"" ELSE 0 END) as ""9下成績""
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
                left outer join (   
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join (
		        select 	
			        ref_student_id
			        ,school_year
			        ,semester
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@A'), '^$', '0') as integer) as ""大功""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@B'), '^$', '0') as integer) as ""小功""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@C'), '^$', '0') as integer) as ""嘉獎""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]') END as ""事由類別""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN ltrim( x1.reason,substring(x1.reason from E'^[\[][^\]]*[\]]')) ELSE x1.reason END as ""事由""
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
			        ,'' as ""事由類別""
			        ,'' as ""事由""
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
	        )  as x1 on student.id=x1.ref_student_id
			    and (
				    (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1)
				    or (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2)
				    or (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1)
				    or (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2)
				    or (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1)
                    or (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2)
			    )
		where 
			student.status = 1 
			and class.grade_year in (3, 9)
		group by student.id
	UNION ALL
		select 
			student.id
			,'大功' as ""科目""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1) THEN x1.""大功"" ELSE 0 END) as ""7上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2) THEN x1.""大功"" ELSE 0 END) as ""7下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1) THEN x1.""大功"" ELSE 0 END) as ""8上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2) THEN x1.""大功"" ELSE 0 END) as ""8下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1) THEN x1.""大功"" ELSE 0 END) as ""9上成績""
            ,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2) THEN x1.""大功"" ELSE 0 END) as ""9下成績""
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
                left outer join (   
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join (
		        select 	
			        ref_student_id
			        ,school_year
			        ,semester
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@A'), '^$', '0') as integer) as ""大功""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@B'), '^$', '0') as integer) as ""小功""
			        ,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@C'), '^$', '0') as integer) as ""嘉獎""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN btrim( substring(x1.reason from E'^[\[][^\]]*[\]]'),'[]') END as ""事由類別""
			        ,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN ltrim( x1.reason,substring(x1.reason from E'^[\[][^\]]*[\]]')) ELSE x1.reason END as ""事由""
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
			        ,'' as ""事由類別""
			        ,'' as ""事由""
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
                    or (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2)
			    )
		where 
			student.status = 1 
			and class.grade_year in (3, 9)
		group by student.id
	UNION ALL
		select student.id
			, '服務學習' as ""科目""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear1 and x1.semester= 1) THEN x1.hours ELSE 0 END) as ""7上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear2 and x1.semester= 2) THEN x1.hours ELSE 0 END) as ""7下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear3 and x1.semester= 1) THEN x1.hours ELSE 0 END) as ""8上成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear4 and x1.semester= 2) THEN x1.hours ELSE 0 END) as ""8下成績""
			,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear5 and x1.semester= 1) THEN x1.hours ELSE 0 END) as ""9上成績""
            ,''||sum(CASE WHEN (''||x1.school_year=shistory.schoolyear6 and x1.semester= 2) THEN x1.hours ELSE 0 END) as ""9下成績""
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
                left outer join (
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
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
		--_$_64 =  $k12.service.learning.record
	UNION ALL
		select student.id
			, '綜合活動' as ""科目""
			,xpath_string('<root>'||x1.score_info||'</root>','/root/Domains/Domain[@領域=''綜合活動'']/@成績') as ""7上成績""
			,xpath_string('<root>'||x2.score_info||'</root>','/root/Domains/Domain[@領域=''綜合活動'']/@成績') as ""7下成績""
			,xpath_string('<root>'||x3.score_info||'</root>','/root/Domains/Domain[@領域=''綜合活動'']/@成績') as ""8上成績""
			,xpath_string('<root>'||x4.score_info||'</root>','/root/Domains/Domain[@領域=''綜合活動'']/@成績') as ""8下成績""
			,xpath_string('<root>'||x5.score_info||'</root>','/root/Domains/Domain[@領域=''綜合活動'']/@成績') as ""9上成績""
            ,xpath_string('<root>'||x6.score_info||'</root>','/root/Domains/Domain[@領域=''綜合活動'']/@成績') as ""9下成績""
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
                left outer join (		
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join sems_subj_score as x1 on student.id=x1.ref_student_id and (''||x1.school_year)=shistory.schoolyear1 and x1.semester= 1
			left join sems_subj_score as x2 on student.id=x2.ref_student_id and (''||x2.school_year)=shistory.schoolyear2 and x2.semester= 2
			left join sems_subj_score as x3 on student.id=x3.ref_student_id and (''||x3.school_year)=shistory.schoolyear3 and x3.semester= 1
			left join sems_subj_score as x4 on student.id=x4.ref_student_id and (''||x4.school_year)=shistory.schoolyear4 and x4.semester= 2
			left join sems_subj_score as x5 on student.id=x5.ref_student_id and (''||x5.school_year)=shistory.schoolyear5 and x5.semester= 1
            left join sems_subj_score as x6 on student.id=x6.ref_student_id and (''||x6.school_year)=shistory.schoolyear6 and x6.semester= 2
	UNION ALL
		select student.id
			, '健康與體育' as ""科目""
			,xpath_string('<root>'||x1.score_info||'</root>','/root/Domains/Domain[@領域=''健康與體育'']/@成績') as ""7上成績""
			,xpath_string('<root>'||x2.score_info||'</root>','/root/Domains/Domain[@領域=''健康與體育'']/@成績') as ""7下成績""
			,xpath_string('<root>'||x3.score_info||'</root>','/root/Domains/Domain[@領域=''健康與體育'']/@成績') as ""8上成績""
			,xpath_string('<root>'||x4.score_info||'</root>','/root/Domains/Domain[@領域=''健康與體育'']/@成績') as ""8下成績""
			,xpath_string('<root>'||x5.score_info||'</root>','/root/Domains/Domain[@領域=''健康與體育'']/@成績') as ""9上成績""
            ,xpath_string('<root>'||x6.score_info||'</root>','/root/Domains/Domain[@領域=''健康與體育'']/@成績') as ""9下成績""
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
                left outer join (		
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join sems_subj_score as x1 on student.id=x1.ref_student_id and (''||x1.school_year)=shistory.schoolyear1 and x1.semester= 1
			left join sems_subj_score as x2 on student.id=x2.ref_student_id and (''||x2.school_year)=shistory.schoolyear2 and x2.semester= 2
			left join sems_subj_score as x3 on student.id=x3.ref_student_id and (''||x3.school_year)=shistory.schoolyear3 and x3.semester= 1
			left join sems_subj_score as x4 on student.id=x4.ref_student_id and (''||x4.school_year)=shistory.schoolyear4 and x4.semester= 2
			left join sems_subj_score as x5 on student.id=x5.ref_student_id and (''||x5.school_year)=shistory.schoolyear5 and x5.semester= 1
            left join sems_subj_score as x6 on student.id=x6.ref_student_id and (''||x6.school_year)=shistory.schoolyear6 and x6.semester= 2
	UNION ALL
		select student.id
			, '藝術與人文' as ""科目""
			,xpath_string('<root>'||x1.score_info||'</root>','/root/Domains/Domain[@領域=''藝術與人文'']/@成績') as ""7上成績""
			,xpath_string('<root>'||x2.score_info||'</root>','/root/Domains/Domain[@領域=''藝術與人文'']/@成績') as ""7下成績""
			,xpath_string('<root>'||x3.score_info||'</root>','/root/Domains/Domain[@領域=''藝術與人文'']/@成績') as ""8上成績""
			,xpath_string('<root>'||x4.score_info||'</root>','/root/Domains/Domain[@領域=''藝術與人文'']/@成績') as ""8下成績""
			,xpath_string('<root>'||x5.score_info||'</root>','/root/Domains/Domain[@領域=''藝術與人文'']/@成績') as ""9上成績""
            ,xpath_string('<root>'||x6.score_info||'</root>','/root/Domains/Domain[@領域=''藝術與人文'']/@成績') as ""9下成績""
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
                left outer join (
					SELECT 
						id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g1 on g1.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g2 on g2.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g3 on g3.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g4 on g4.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g5 on g5.id=student.id 
				left outer join (
					SELECT id, max(SchoolYear) as SchoolYear 
					FROM xpath_table( 
						'id'
						, '''<root>''||sems_history||''</root>'''
						, 'student'
						, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
						, 'id IN ( 
							select 
								student.id 
							from 
								student 
								LEFT OUTER JOIN class ON student.ref_class_id = class.id 
							WHERE student.status=1 AND class.grade_year in (3, 9) )'
						) AS tmp(id int, SchoolYear integer) 
					group by id 
				)as g6 on g6.id=student.id 
				LEFT OUTER JOIN class ON student.ref_class_id = class.id 
				WHERE student.status=1 AND class.grade_year in (3, 9)
            )shistory on student.id=shistory.id
			left join sems_subj_score as x1 on student.id=x1.ref_student_id and (''||x1.school_year)=shistory.schoolyear1 and x1.semester= 1
			left join sems_subj_score as x2 on student.id=x2.ref_student_id and (''||x2.school_year)=shistory.schoolyear2 and x2.semester= 2
			left join sems_subj_score as x3 on student.id=x3.ref_student_id and (''||x3.school_year)=shistory.schoolyear3 and x3.semester= 1
			left join sems_subj_score as x4 on student.id=x4.ref_student_id and (''||x4.school_year)=shistory.schoolyear4 and x4.semester= 2
			left join sems_subj_score as x5 on student.id=x5.ref_student_id and (''||x5.school_year)=shistory.schoolyear5 and x5.semester= 1
            left join sems_subj_score as x6 on student.id=x6.ref_student_id and (''||x6.school_year)=shistory.schoolyear6 and x6.semester= 2
	)s1 on s1.id = student.id
where student.status = 1 and class.grade_year in (3, 9)
order by class.display_order, class_name, seat_no
";

        #endregion

        #region 獎懲記錄IncentiveRecord
        public static string IncentiveRecord = @"
select (
	    select 
		    xpath_string(content,'/SchoolInformation/Code') 
	    from list 
	    where name = '學校資訊'
	)as ""國中學校代碼""
	, substr(class.class_name, length(class.class_name)-1, 2) as ""班級""
	, student.id_number as ""身分證字號""
	,CASE 
		WHEN (x1.school_year=shistory.schoolyear1 and x1.semester= 1) THEN '7上'
		WHEN (x1.school_year=shistory.schoolyear2 and x1.semester= 2) THEN '7下'
		WHEN (x1.school_year=shistory.schoolyear3 and x1.semester= 1) THEN '8上'
		WHEN (x1.school_year=shistory.schoolyear4 and x1.semester= 2) THEN '8下'
		WHEN (x1.school_year=shistory.schoolyear5 and x1.semester= 1) THEN '9上'
		WHEN (x1.school_year=shistory.schoolyear6 and x1.semester= 2) THEN '9下'
	END
		as ""學期""
	,x1.""大功""
	,x1.""小功""
	,x1.""嘉獎""
	,x1.""事由類別""
	,x1.""事由""
from
	student
	left outer join class on student.ref_class_id=class.id
	left join (
		SELECT student.id
	        , g1.SchoolYear as schoolyear1
	        , g2.SchoolYear as schoolyear2
	        , g3.SchoolYear as schoolyear3
	        , g4.SchoolYear as schoolyear4
	        , g5.SchoolYear as schoolyear5
            , g6.SchoolYear as schoolyear6
        FROM student
        left outer join (	
			SELECT 
				id, max(SchoolYear) as SchoolYear 
			FROM xpath_table( 
				'id'
				, '''<root>''||sems_history||''</root>'''
				, 'student'
				, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''1'')]/@SchoolYear'
				, 'id IN ( 
					select 
						student.id 
					from 
						student 
						LEFT OUTER JOIN class ON student.ref_class_id = class.id 
					WHERE student.status=1 AND class.grade_year in (3, 9) )'
				) AS tmp(id int, SchoolYear integer) 
			group by id 
		)as g1 on g1.id=student.id 
		left outer join (
			SELECT id, max(SchoolYear) as SchoolYear 
			FROM xpath_table( 
				'id'
				, '''<root>''||sems_history||''</root>'''
				, 'student'
				, '/root/History[ ( @GradeYear=''7'' or @GradeYear=''1'' ) and (@Semester=''2'')]/@SchoolYear'
				, 'id IN ( 
					select 
						student.id 
					from 
						student 
						LEFT OUTER JOIN class ON student.ref_class_id = class.id 
					WHERE student.status=1 AND class.grade_year in (3, 9) )'
				) AS tmp(id int, SchoolYear integer) 
			group by id 
		)as g2 on g2.id=student.id 
		left outer join (
			SELECT id, max(SchoolYear) as SchoolYear 
			FROM xpath_table( 
				'id'
				, '''<root>''||sems_history||''</root>'''
				, 'student'
				, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''1'')]/@SchoolYear'
				, 'id IN ( 
					select 
						student.id 
					from 
						student 
						LEFT OUTER JOIN class ON student.ref_class_id = class.id 
					WHERE student.status=1 AND class.grade_year in (3, 9) )'
				) AS tmp(id int, SchoolYear integer) 
			group by id 
		)as g3 on g3.id=student.id 
		left outer join (
			SELECT id, max(SchoolYear) as SchoolYear 
			FROM xpath_table( 
				'id'
				, '''<root>''||sems_history||''</root>'''
				, 'student'
				, '/root/History[ ( @GradeYear=''8'' or @GradeYear=''2'' ) and (@Semester=''2'')]/@SchoolYear'
				, 'id IN ( 
					select 
						student.id 
					from 
						student 
						LEFT OUTER JOIN class ON student.ref_class_id = class.id 
					WHERE student.status=1 AND class.grade_year in (3, 9) )'
				) AS tmp(id int, SchoolYear integer) 
			group by id 
		)as g4 on g4.id=student.id 
		left outer join (
			SELECT id, max(SchoolYear) as SchoolYear 
			FROM xpath_table( 
				'id'
				, '''<root>''||sems_history||''</root>'''
				, 'student'
				, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''1'')]/@SchoolYear'
				, 'id IN ( 
					select 
						student.id 
					from 
						student 
						LEFT OUTER JOIN class ON student.ref_class_id = class.id 
					WHERE student.status=1 AND class.grade_year in (3, 9) )'
				) AS tmp(id int, SchoolYear integer) 
			group by id 
		)as g5 on g5.id=student.id 
		left outer join (
			SELECT id, max(SchoolYear) as SchoolYear 
			FROM xpath_table( 
				'id'
				, '''<root>''||sems_history||''</root>'''
				, 'student'
				, '/root/History[ ( @GradeYear=''9'' or @GradeYear=''3'' ) and (@Semester=''2'')]/@SchoolYear'
				, 'id IN ( 
					select 
						student.id 
					from 
						student 
						LEFT OUTER JOIN class ON student.ref_class_id = class.id 
					WHERE student.status=1 AND class.grade_year in (3, 9) )'
				) AS tmp(id int, SchoolYear integer) 
			group by id 
		)as g6 on g6.id=student.id 
		LEFT OUTER JOIN class ON student.ref_class_id = class.id 
		WHERE student.status=1 AND class.grade_year in (3, 9)
	)shistory on student.id=shistory.id
	left outer join (
		select 	
			ref_student_id
			,school_year
			,semester
			,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@A'), '^$', '0') as integer) as ""大功""
			,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@B'), '^$', '0') as integer) as ""小功""
			,CAST( regexp_replace( xpath_string(x1.detail,'/Discipline/Merit/@C'), '^$', '0') as integer) as ""嘉獎""
			,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN substring(x1.reason, 2, position( ']' in x1.reason )-2) END as ""事由類別""		
			,CASE WHEN x1.reason ~ E'^[\[][^\]]*[\]]' THEN substring(x1.reason, position( ']' in x1.reason ) + 1 ) ELSE x1.reason END as ""事由""
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
			,'' as ""事由類別""
			,school_year || '-' || semester || '獎勵統計' as ""事由""
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
	)as x1 on student.id=x1.ref_student_id
where 
	student.status = 1 
	and class.grade_year in (3, 9)
	and (
		(x1.school_year=shistory.schoolyear1 and x1.semester= 1)
		or (x1.school_year=shistory.schoolyear2 and x1.semester= 2)
		or (x1.school_year=shistory.schoolyear3 and x1.semester= 1)
		or (x1.school_year=shistory.schoolyear4 and x1.semester= 2)
		or (x1.school_year=shistory.schoolyear5 and x1.semester= 1)
		or (x1.school_year=shistory.schoolyear6 and x1.semester= 2)
	)
order by class.display_order, class.class_name, seat_no, 身分證字號, 學期;
";
        #endregion
    }
}