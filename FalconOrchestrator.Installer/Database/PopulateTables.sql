insert into [dbo].[Status] values ('Open','Detection is in the queue for review and has not yet been looked at by an analyst'); 
insert into [dbo].[Status] values ('Triage','Actively being investigated by the assigned analyst');
insert into [dbo].[Status] values ('Containment','Has been contained and is no longer an active threat to the environment');
insert into [dbo].[Status] values ('Remediation','Remedial action is underway');
insert into [dbo].[Status] values ('False Positive', 'Detection is a false positive and not a legitimate security concern');
insert into [dbo].[Status] values ('Closed','Event has been reviewed and closed, more details in commentary');
insert into [dbo].[Status] values ('Whitelisted','Event matches a whitelisting rule');

insert into [dbo].[Severities] values ('Informational','Something is there that probably should not be, but dont lose sleep over it');
insert into [dbo].[Severities] values ('Low','Low priority, likely adware or something that is not an immediate security threat');
insert into [dbo].[Severities] values ('Medium','Less urgent but should be reviewed');
insert into [dbo].[Severities] values ('High','Strong probability this is a valid alert of either well known malware executing or supsicious behaviors being observed')
insert into [dbo].[Severities] values ('Critical','Alert has a very high probability of being an active attack, if you see this, wake up your IR team');

insert into [dbo].[ResponderSchedules] values ('Monday',null);
insert into [dbo].[ResponderSchedules] values ('Tuesday',null);
insert into [dbo].[ResponderSchedules] values ('Wednesday',null);
insert into [dbo].[ResponderSchedules] values ('Thursday',null);
insert into [dbo].[ResponderSchedules] values ('Friday',null);
insert into [dbo].[ResponderSchedules] values ('Saturday',null);
insert into [dbo].[ResponderSchedules] values ('Sunday',null);

insert into [dbo].[TaxonomyTypes] values ('Username') 
insert into [dbo].[TaxonomyTypes] values ('Hostname')
insert into [dbo].[TaxonomyTypes] values ('Active Directory OU')
insert into [dbo].[TaxonomyTypes] values ('Active Directory Group')

insert into [dbo].[WhitelistTypes] values ('SHA256')
insert into [dbo].[WhitelistTypes] values ('File Name')
insert into [dbo].[WhitelistTypes] values ('File Path')
insert into [dbo].[WhitelistTypes] values ('Command Line')

insert into [dbo].[Configurations] values ('FALCON_STREAM_URL','https://firehose.crowdstrike.com/sensors/entities/datafeed/v1')
insert into [dbo].[Configurations] values('FALCON_STREAM_LAST_OFFSET','0')
insert into [dbo].[Configurations] values ('FALCON_QUERY_URL','https://falconapi.crowdstrike.com/')

insert into [dbo].[Configurations] values('LDAP_DAYS_VALID','30')
insert into [dbo].[Configurations] values('RULE_AD_LOOKUP','False')
insert into [dbo].[Configurations] values('RULE_WHITELISTING','False')
insert into [dbo].[Configurations] values('RULE_TAXONOMIZE','False')
insert into [dbo].[Configurations] values('RULE_DNS_LOOKUP','False')
insert into [dbo].[Configurations] values('RULE_ASSIGN_HANDLER','False')
insert into [dbo].[Configurations] values('RULE_NOTIFICATION','False')
insert into [dbo].[Configurations] values('EMAIL_SSL','True')
insert into [dbo].[Configurations] values('EMAIL_ALERT_SUBJECT','{{Severity}} Severity Detection On {{Hostname}}\{{Username}}')
insert into [dbo].[Configurations] values('EMAIL_TICKET_SUBJECT','A {{Priority}} Priority Ticket Has been Opened For {{Hostname}}\{{Username}}')
insert into [dbo].[Configurations] values('EMAIL_TICKET_TEMPLATE_PATH','C:\Inetpub\Falcon Orchestrator\App_Data\Templates\ticket_template.html')
insert into [dbo].[Configurations] values('EMAIL_ALERT_TEMPLATE_PATH','C:\Inetpub\Falcon Orchestrator\App_Data\Templates\alert_template.html')

GO

