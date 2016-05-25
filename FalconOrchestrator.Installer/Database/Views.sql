IF OBJECT_ID(N'[dbo].[v_DetectionCountByDay]') IS NOT NULL
    DROP VIEW [dbo].[v_DetectionCountByDay];
GO
CREATE VIEW [dbo].[v_DetectionCountByDay] AS
select dr.IndividualDate, count(DetectionId) as CountPerDay 
	from (select IndividualDate from fn_DetectionDateRange()) as dr
	left join Detections d on dr.IndividualDate=convert(date,d.ProcessStartTime)
	group by dr.IndividualDate
GO


IF OBJECT_ID(N'[dbo].[v_DetectionAverages]') IS NOT NULL
    DROP VIEW [dbo].[v_DetectionAverages];
GO
CREATE VIEW [dbo].[v_DetectionAverages] AS
select 
Id = 1,
Daily = (select avg(CountPerDay)as DailyAvg from v_DetectionCountByDay),
Weekly = (select avg(CountPerWeek) as WeeklyAvg 
from
(
	select sum(CountPerDay) as CountPerWeek from v_DetectionCountByDay 
	group by datepart(WEEK,IndividualDate)
) groupedByWeek),
Monthly = (select avg(CountPerMonth) as MonthlyAvg 
from
(
	select sum(CountPerDay) as CountPerMonth from v_DetectionCountByDay 
	group by datepart(MONTH,IndividualDate)
) groupedByMonth)
GO