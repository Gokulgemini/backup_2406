USE [msdb]
GO

-- delete all schedules and add the 2 below
DECLARE @jobschedules TABLE (schedule_id INT)
DECLARE @job_id UNIQUEIDENTIFIER
	,@schedule_id INT

SELECT @job_id = job_id
FROM sysjobs
WHERE name = 'Coroner-ImageVault'

INSERT INTO @jobschedules (schedule_id)
SELECT s.schedule_id
FROM sysjobs j
INNER JOIN sysjobschedules sjs ON j.job_id = sjs.job_id
INNER JOIN sysschedules s ON sjs.schedule_id = s.schedule_id
WHERE j.job_id = @job_id

SELECT TOP 1 @schedule_id = schedule_id
FROM @jobschedules

WHILE @schedule_id IS NOT NULL
BEGIN
	EXEC msdb.dbo.sp_detach_schedule @job_id = @job_id
		,@schedule_id = @schedule_id
		,@delete_unused_schedule = 1

	DELETE js
	FROM @jobschedules js
	WHERE schedule_id = @schedule_id

	SET @schedule_id = NULL

	SELECT TOP 1 @schedule_id = schedule_id
	FROM @jobschedules
END

-- Add new schedule
EXEC msdb.dbo.sp_add_jobschedule @job_id = @job_id
	,@name = N'12:00 AM to 08:00 AM'
	,@enabled = 1
	,@freq_type = 4
	,@freq_interval = 1
	,@freq_subday_type = 4
	,@freq_subday_interval = 1
	,@freq_relative_interval = 0
	,@freq_recurrence_factor = 1
	,@active_start_date = 20200615
	,@active_end_date = 99991231
	,@active_start_time = 0
	,@active_end_time = 80000
	,@schedule_id = @schedule_id OUTPUT

EXEC msdb.dbo.sp_add_jobschedule @job_id = @job_id
	,@name = N'08:00 PM to 11:59 PM'
	,@enabled = 1
	,@freq_type = 4
	,@freq_interval = 1
	,@freq_subday_type = 4
	,@freq_subday_interval = 1
	,@freq_relative_interval = 0
	,@freq_recurrence_factor = 1
	,@active_start_date = 20200615
	,@active_end_date = 99991231
	,@active_start_time = 200000
	,@active_end_time = 235959
	,@schedule_id = @schedule_id OUTPUT
GO
