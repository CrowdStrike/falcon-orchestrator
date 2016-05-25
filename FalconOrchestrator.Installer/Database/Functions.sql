IF OBJECT_ID(N'[dbo].[fn_DateRange]') IS NOT NULL
    DROP FUNCTION [dbo].[fn_DateRange];
GO
CREATE FUNCTION [dbo].[fn_DateRange]
(     
      @Increment              CHAR(1),
      @StartDate              DATETIME,
      @EndDate                DATETIME
)
RETURNS @SelectedRange TABLE 
(
	IndividualDate DATETIME
)
AS 
BEGIN
      ;WITH cteRange (DateRange) AS (
            SELECT @StartDate
            UNION ALL
            SELECT 
                  CASE
                        WHEN @Increment = 'd' THEN DATEADD(dd, 1, DateRange)
                        WHEN @Increment = 'w' THEN DATEADD(ww, 1, DateRange)
                        WHEN @Increment = 'm' THEN DATEADD(mm, 1, DateRange)
                  END
            FROM cteRange
            WHERE DateRange <= 
                  CASE
                        WHEN @Increment = 'd' THEN DATEADD(dd, -1, @EndDate)
                        WHEN @Increment = 'w' THEN DATEADD(ww, -1, @EndDate)
                        WHEN @Increment = 'm' THEN DATEADD(mm, -1, @EndDate)
                  END)
          
      INSERT INTO @SelectedRange (IndividualDate)
      SELECT DateRange
      FROM cteRange
      OPTION (MAXRECURSION 3660);
      RETURN
END
GO

IF OBJECT_ID(N'[dbo].[fn_DetectionDateRange]') IS NOT NULL
    DROP FUNCTION [dbo].[fn_DetectionDateRange];
GO
CREATE FUNCTION [dbo].[fn_DetectionDateRange]()
RETURNS @ReturnTable TABLE
(
	IndividualDate DATETIME
)
AS
BEGIN
	declare @FirstDate datetime, @LastDate datetime
	select @FirstDate = min(convert(date,ProcessStartTime)), @LastDate = max(convert(date,ProcessStartTime)) from Detections
	insert into @ReturnTable (IndividualDate)
	select IndividualDate from fn_DateRange('d',@FirstDate,@LastDate)
RETURN
END
GO

