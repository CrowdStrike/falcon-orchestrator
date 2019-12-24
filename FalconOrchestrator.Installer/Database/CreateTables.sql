
-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AccountGroups'
CREATE TABLE [dbo].[AccountGroups] (
    [AccountGroupId] int IDENTITY(1,1) NOT NULL,
    [AccountId] int  NOT NULL,
    [GroupId] int  NOT NULL
);
GO

-- Creating table 'Accounts'
CREATE TABLE [dbo].[Accounts] (
    [AccountId] int IDENTITY(1,1) NOT NULL,
    [AccountName] nvarchar(50)  NOT NULL,
    [FirstName] nvarchar(50)  NULL,
    [LastName] nvarchar(50)  NULL,
    [Department] nvarchar(50)  NULL,
    [JobTitle] nvarchar(50)  NULL,
    [Manager] nvarchar(75)  NULL,
    [EmailAddress] nvarchar(75)  NULL,
    [PhoneNumber] nvarchar(20)  NULL,
    [Country] nvarchar(50)  NULL,
    [StateProvince] nvarchar(50)  NULL,
    [City] nvarchar(50)  NULL,
    [StreetAddress] nvarchar(75)  NULL,
    [LastLogon] datetime  NULL,
    [OrganizationalUnit] nvarchar(max)  NULL,
    [Timestamp] datetime  NOT NULL
);
GO

-- Creating table 'AccountTickets'
CREATE TABLE [dbo].[AccountTickets] (
    [AccounTicketId] int IDENTITY(1,1) NOT NULL,
    [AccountId] int  NOT NULL,
    [TicketId] int  NOT NULL
);
GO

-- Creating table 'AuthenticationLogs'
CREATE TABLE [dbo].[AuthenticationLogs] (
    [AuthId] int IDENTITY(1,1) NOT NULL,
    [OperationName] nvarchar(50)  NOT NULL,
    [ServiceName] nvarchar(50)  NOT NULL,
    [Success] bit  NOT NULL,
    [UserId] nvarchar(50)  NULL,
    [UserIp] nvarchar(45)  NULL,
    [Timestamp] datetime  NOT NULL,
    [Offset] nvarchar(max)  NOT NULL,
    [Entitlement] nvarchar(100)  NULL,
    [EntitlementGroup] nvarchar(100)  NULL,
    [TargetName] nvarchar(200)  NULL,
    [CustomerId] int  NOT NULL
);
GO

-- Creating table 'Configurations'
CREATE TABLE [dbo].[Configurations] (
    [ConfigId] int IDENTITY(1,1) NOT NULL,
    [Key] nvarchar(max)  NOT NULL,
    [Value] nvarchar(max)  NULL
);
GO

-- Creating table 'CustomerIOCEvents'
CREATE TABLE [dbo].[CustomerIOCEvents] (
    [CustomIOCEventId] int IDENTITY(1,1) NOT NULL,
    [AgentIdString] nvarchar(max)  NOT NULL,
    [Offset] bigint  NOT NULL,
    [IOCType] nvarchar(50)  NOT NULL,
    [IOCValue] nvarchar(max)  NOT NULL,
    [CustomerId] int  NOT NULL,
    [DetectionDeviceId] int  NOT NULL
);
GO

-- Creating table 'Customers'
CREATE TABLE [dbo].[Customers] (
    [CustomerId] int IDENTITY(1,1) NOT NULL,
    [CustomerIdString] nvarchar(max)  NOT NULL,
    [Name] nvarchar(100)  NULL
);
GO

-- Creating table 'DetectionDevices'
CREATE TABLE [dbo].[DetectionDevices] (
    [DetectionDeviceId] int IDENTITY(1,1) NOT NULL,
    [IPAddress] nvarchar(45)  NULL,
    [DeviceId] int  NOT NULL
);
GO

-- Creating table 'Detections'
CREATE TABLE [dbo].[Detections] (
    [DetectionId] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [ProcessStartTime] datetime  NULL,
    [ProcessEndTime] datetime  NULL,
    [CommandLine] nvarchar(max)  NULL,
    [FilePath] nvarchar(max)  NOT NULL,
    [FileName] nvarchar(max)  NOT NULL,
    [FalconHostLink] nvarchar(500)  NOT NULL,
    [Description] nvarchar(200)  NULL,
    [SHA256] nvarchar(64)  NOT NULL,
    [MD5] nvarchar(32)  NOT NULL,
    [SHA1] nvarchar(40)  NOT NULL,
    [AVDetections] bit  NULL,
    [Comment] nvarchar(max)  NULL,
    [ClosedDate] datetime  NULL,
    [AccountId] int  NOT NULL,
    [DetectionDeviceId] int  NOT NULL,
    [Offset] nvarchar(max)  NOT NULL,
    [ProcessId] nvarchar(100)  NULL,
    [ParentProcessId] nvarchar(100)  NOT NULL,
    [StatusId] int  NOT NULL,
    [VendorSeverityId] int  NOT NULL,
    [CustomerId] int  NOT NULL,
    [CustomSeverityId] int  NOT NULL,
    [ResponderId] int  NULL
);
GO

-- Creating table 'DetectionTags'
CREATE TABLE [dbo].[DetectionTags] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [DetectionId] int  NOT NULL,
    [TagId] int  NOT NULL
);
GO

-- Creating table 'DetectionTaxonomies'
CREATE TABLE [dbo].[DetectionTaxonomies] (
    [DetectTaxId] int IDENTITY(1,1) NOT NULL,
    [DetectionId] int  NOT NULL,
    [TaxonomyId] int  NOT NULL
);
GO

-- Creating table 'DetectionTickets'
CREATE TABLE [dbo].[DetectionTickets] (
    [DetectionTicketId] int IDENTITY(1,1) NOT NULL,
    [DetectionId] int  NOT NULL,
    [TicketId] int  NOT NULL
);
GO

-- Creating table 'Devices'
CREATE TABLE [dbo].[Devices] (
    [DeviceId] int IDENTITY(1,1) NOT NULL,
    [Hostname] nvarchar(50)  NOT NULL,
    [Domain] nvarchar(50)  NULL,
    [SensorId] nvarchar(100)  NULL
);
GO

-- Creating table 'DeviceTickets'
CREATE TABLE [dbo].[DeviceTickets] (
    [DeviceTicketId] int IDENTITY(1,1) NOT NULL,
    [DeviceId] int  NOT NULL,
    [TicketId] int  NOT NULL
);
GO

-- Creating table 'DnsRequests'
CREATE TABLE [dbo].[DnsRequests] (
    [DnsRequestId] int IDENTITY(1,1) NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [CausedDetect] bit  NOT NULL,
    [DomainName] nvarchar(500)  NOT NULL,
    [RequestType] nvarchar(50)  NOT NULL,
    [InterfaceIndex] int  NOT NULL,
    [DetectionId] int  NOT NULL
);
GO

-- Creating table 'DocumentsAccesses'
CREATE TABLE [dbo].[DocumentsAccesses] (
    [DocAccessedId] int IDENTITY(1,1) NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [FileName] nvarchar(max)  NOT NULL,
    [FilePath] nvarchar(max)  NOT NULL,
    [DetectionId] int  NOT NULL
);
GO

-- Creating table 'ExecutablesWrittens'
CREATE TABLE [dbo].[ExecutablesWrittens] (
    [ExeWrittenId] int IDENTITY(1,1) NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [FileName] nvarchar(max)  NOT NULL,
    [FilePath] nvarchar(max)  NOT NULL,
    [DetectionId] int  NOT NULL
);
GO

-- Creating table 'Groups'
CREATE TABLE [dbo].[Groups] (
    [GroupId] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'NetworkAccesses'
CREATE TABLE [dbo].[NetworkAccesses] (
    [NetAccessId] int IDENTITY(1,1) NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [AccessType] int  NOT NULL,
    [ConnectionDirection] int  NOT NULL,
    [IsIPv6] bit  NOT NULL,
    [LocalAddress] nvarchar(45)  NOT NULL,
    [LocalPort] int  NOT NULL,
    [RemoteAddress] nvarchar(45)  NOT NULL,
    [RemotePort] int  NOT NULL,
    [Protocol] nvarchar(20)  NOT NULL,
    [DetectionId] int  NOT NULL
);
GO

-- Creating table 'Responders'
CREATE TABLE [dbo].[Responders] (
    [ResponderId] int IDENTITY(1,1) NOT NULL,
    [FirstName] nvarchar(50)  NOT NULL,
    [LastName] nvarchar(50)  NOT NULL,
    [EmailAddress] nvarchar(255)  NOT NULL,
    [PhoneNumber] nvarchar(25)  NULL
);
GO

-- Creating table 'ResponderSchedules'
CREATE TABLE [dbo].[ResponderSchedules] (
    [ScheduleId] int IDENTITY(1,1) NOT NULL,
    [DayOfWeek] nvarchar(50)  NOT NULL,
    [ResponderId] int  NULL
);
GO

-- Creating table 'ScanResults'
CREATE TABLE [dbo].[ScanResults] (
    [ScanResultId] int IDENTITY(1,1) NOT NULL,
    [Engine] nvarchar(100)  NOT NULL,
    [ResultName] nvarchar(200)  NOT NULL,
    [Version] nvarchar(50)  NOT NULL,
    [DetectionId] int  NOT NULL
);
GO

-- Creating table 'Severities'
CREATE TABLE [dbo].[Severities] (
    [SeverityId] int IDENTITY(1,1) NOT NULL,
    [SeverityType] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'Status'
CREATE TABLE [dbo].[Status] (
    [StatusId] int IDENTITY(1,1) NOT NULL,
    [StatusType] nvarchar(25)  NOT NULL,
    [Description] nvarchar(max)  NULL
);
GO

-- Creating table 'Tags'
CREATE TABLE [dbo].[Tags] (
    [TagId] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'Taxonomies'
CREATE TABLE [dbo].[Taxonomies] (
    [TaxonomyId] int IDENTITY(1,1) NOT NULL,
    [Creator] nvarchar(100)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [Critical] bit  NOT NULL,
    [Value] nvarchar(max)  NOT NULL,
    [TaxTypeId] int  NOT NULL
);
GO

-- Creating table 'TaxonomyTypes'
CREATE TABLE [dbo].[TaxonomyTypes] (
    [TaxTypeId] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(100)  NOT NULL
);
GO

-- Creating table 'TicketRecipients'
CREATE TABLE [dbo].[TicketRecipients] (
    [TicketRecipientId] int IDENTITY(1,1) NOT NULL,
    [Title] nvarchar(100)  NOT NULL,
    [EmailAddress] nvarchar(255)  NOT NULL,
    [PhoneNumber] nvarchar(20)  NULL
);
GO

-- Creating table 'Tickets'
CREATE TABLE [dbo].[Tickets] (
    [TicketId] int IDENTITY(1,1) NOT NULL,
    [ExternalTicket] nvarchar(20)  NULL,
    [Creator] nvarchar(50)  NOT NULL,
    [DispatchDate] datetime  NULL,
    [CompletionDate] datetime  NULL,
    [TicketRecipientId] int  NOT NULL,
    [Comment] nvarchar(max)  NULL,
    [SeverityId] int  NOT NULL
);
GO

-- Creating table 'UserActivityLogs'
CREATE TABLE [dbo].[UserActivityLogs] (
    [UserActivityLogId] int IDENTITY(1,1) NOT NULL,
    [OperationName] nvarchar(50)  NOT NULL,
    [ServiceName] nvarchar(50)  NOT NULL,
    [Success] bit  NOT NULL,
    [UserId] nvarchar(50)  NULL,
    [UserIp] nvarchar(45)  NULL,
    [Timestamp] datetime  NOT NULL,
    [Offset] nvarchar(max)  NOT NULL,
    [DetectId] nvarchar(max)  NULL,
    [State] nvarchar(100)  NULL,
    [CustomerId] int  NOT NULL
);
GO

-- Creating table 'Whitelists'
CREATE TABLE [dbo].[Whitelists] (
    [WhitelistId] int IDENTITY(1,1) NOT NULL,
    [Creator] nvarchar(100)  NOT NULL,
    [Reason] nvarchar(max)  NOT NULL,
    [Timestamp] datetime  NOT NULL,
    [WhitelistTypeId] int  NOT NULL,
    [Value] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'WhitelistTypes'
CREATE TABLE [dbo].[WhitelistTypes] (
    [WhitelistTypeId] int IDENTITY(1,1) NOT NULL,
    [Type] nvarchar(100)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [AccountGroupId] in table 'AccountGroups'
ALTER TABLE [dbo].[AccountGroups]
ADD CONSTRAINT [PK_AccountGroups]
    PRIMARY KEY CLUSTERED ([AccountGroupId] ASC);
GO

-- Creating primary key on [AccountId] in table 'Accounts'
ALTER TABLE [dbo].[Accounts]
ADD CONSTRAINT [PK_Accounts]
    PRIMARY KEY CLUSTERED ([AccountId] ASC);
GO

-- Creating primary key on [AccounTicketId] in table 'AccountTickets'
ALTER TABLE [dbo].[AccountTickets]
ADD CONSTRAINT [PK_AccountTickets]
    PRIMARY KEY CLUSTERED ([AccounTicketId] ASC);
GO

-- Creating primary key on [AuthId] in table 'AuthenticationLogs'
ALTER TABLE [dbo].[AuthenticationLogs]
ADD CONSTRAINT [PK_AuthenticationLogs]
    PRIMARY KEY CLUSTERED ([AuthId] ASC);
GO

-- Creating primary key on [ConfigId] in table 'Configurations'
ALTER TABLE [dbo].[Configurations]
ADD CONSTRAINT [PK_Configurations]
    PRIMARY KEY CLUSTERED ([ConfigId] ASC);
GO

-- Creating primary key on [CustomIOCEventId] in table 'CustomerIOCEvents'
ALTER TABLE [dbo].[CustomerIOCEvents]
ADD CONSTRAINT [PK_CustomerIOCEvents]
    PRIMARY KEY CLUSTERED ([CustomIOCEventId] ASC);
GO

-- Creating primary key on [CustomerId] in table 'Customers'
ALTER TABLE [dbo].[Customers]
ADD CONSTRAINT [PK_Customers]
    PRIMARY KEY CLUSTERED ([CustomerId] ASC);
GO

-- Creating primary key on [DetectionDeviceId] in table 'DetectionDevices'
ALTER TABLE [dbo].[DetectionDevices]
ADD CONSTRAINT [PK_DetectionDevices]
    PRIMARY KEY CLUSTERED ([DetectionDeviceId] ASC);
GO

-- Creating primary key on [DetectionId] in table 'Detections'
ALTER TABLE [dbo].[Detections]
ADD CONSTRAINT [PK_Detections]
    PRIMARY KEY CLUSTERED ([DetectionId] ASC);
GO

-- Creating primary key on [Id] in table 'DetectionTags'
ALTER TABLE [dbo].[DetectionTags]
ADD CONSTRAINT [PK_DetectionTags]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [DetectTaxId] in table 'DetectionTaxonomies'
ALTER TABLE [dbo].[DetectionTaxonomies]
ADD CONSTRAINT [PK_DetectionTaxonomies]
    PRIMARY KEY CLUSTERED ([DetectTaxId] ASC);
GO

-- Creating primary key on [DetectionTicketId] in table 'DetectionTickets'
ALTER TABLE [dbo].[DetectionTickets]
ADD CONSTRAINT [PK_DetectionTickets]
    PRIMARY KEY CLUSTERED ([DetectionTicketId] ASC);
GO

-- Creating primary key on [DeviceId] in table 'Devices'
ALTER TABLE [dbo].[Devices]
ADD CONSTRAINT [PK_Devices]
    PRIMARY KEY CLUSTERED ([DeviceId] ASC);
GO

-- Creating primary key on [DeviceTicketId] in table 'DeviceTickets'
ALTER TABLE [dbo].[DeviceTickets]
ADD CONSTRAINT [PK_DeviceTickets]
    PRIMARY KEY CLUSTERED ([DeviceTicketId] ASC);
GO

-- Creating primary key on [DnsRequestId] in table 'DnsRequests'
ALTER TABLE [dbo].[DnsRequests]
ADD CONSTRAINT [PK_DnsRequests]
    PRIMARY KEY CLUSTERED ([DnsRequestId] ASC);
GO

-- Creating primary key on [DocAccessedId] in table 'DocumentsAccesses'
ALTER TABLE [dbo].[DocumentsAccesses]
ADD CONSTRAINT [PK_DocumentsAccesses]
    PRIMARY KEY CLUSTERED ([DocAccessedId] ASC);
GO

-- Creating primary key on [ExeWrittenId] in table 'ExecutablesWrittens'
ALTER TABLE [dbo].[ExecutablesWrittens]
ADD CONSTRAINT [PK_ExecutablesWrittens]
    PRIMARY KEY CLUSTERED ([ExeWrittenId] ASC);
GO

-- Creating primary key on [GroupId] in table 'Groups'
ALTER TABLE [dbo].[Groups]
ADD CONSTRAINT [PK_Groups]
    PRIMARY KEY CLUSTERED ([GroupId] ASC);
GO

-- Creating primary key on [NetAccessId] in table 'NetworkAccesses'
ALTER TABLE [dbo].[NetworkAccesses]
ADD CONSTRAINT [PK_NetworkAccesses]
    PRIMARY KEY CLUSTERED ([NetAccessId] ASC);
GO

-- Creating primary key on [ResponderId] in table 'Responders'
ALTER TABLE [dbo].[Responders]
ADD CONSTRAINT [PK_Responders]
    PRIMARY KEY CLUSTERED ([ResponderId] ASC);
GO

-- Creating primary key on [ScheduleId] in table 'ResponderSchedules'
ALTER TABLE [dbo].[ResponderSchedules]
ADD CONSTRAINT [PK_ResponderSchedules]
    PRIMARY KEY CLUSTERED ([ScheduleId] ASC);
GO

-- Creating primary key on [ScanResultId] in table 'ScanResults'
ALTER TABLE [dbo].[ScanResults]
ADD CONSTRAINT [PK_ScanResults]
    PRIMARY KEY CLUSTERED ([ScanResultId] ASC);
GO

-- Creating primary key on [SeverityId] in table 'Severities'
ALTER TABLE [dbo].[Severities]
ADD CONSTRAINT [PK_Severities]
    PRIMARY KEY CLUSTERED ([SeverityId] ASC);
GO

-- Creating primary key on [StatusId] in table 'Status'
ALTER TABLE [dbo].[Status]
ADD CONSTRAINT [PK_Status]
    PRIMARY KEY CLUSTERED ([StatusId] ASC);
GO

-- Creating primary key on [TagId] in table 'Tags'
ALTER TABLE [dbo].[Tags]
ADD CONSTRAINT [PK_Tags]
    PRIMARY KEY CLUSTERED ([TagId] ASC);
GO

-- Creating primary key on [TaxonomyId] in table 'Taxonomies'
ALTER TABLE [dbo].[Taxonomies]
ADD CONSTRAINT [PK_Taxonomies]
    PRIMARY KEY CLUSTERED ([TaxonomyId] ASC);
GO

-- Creating primary key on [TaxTypeId] in table 'TaxonomyTypes'
ALTER TABLE [dbo].[TaxonomyTypes]
ADD CONSTRAINT [PK_TaxonomyTypes]
    PRIMARY KEY CLUSTERED ([TaxTypeId] ASC);
GO

-- Creating primary key on [TicketRecipientId] in table 'TicketRecipients'
ALTER TABLE [dbo].[TicketRecipients]
ADD CONSTRAINT [PK_TicketRecipients]
    PRIMARY KEY CLUSTERED ([TicketRecipientId] ASC);
GO

-- Creating primary key on [TicketId] in table 'Tickets'
ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [PK_Tickets]
    PRIMARY KEY CLUSTERED ([TicketId] ASC);
GO

-- Creating primary key on [UserActivityLogId] in table 'UserActivityLogs'
ALTER TABLE [dbo].[UserActivityLogs]
ADD CONSTRAINT [PK_UserActivityLogs]
    PRIMARY KEY CLUSTERED ([UserActivityLogId] ASC);
GO

-- Creating primary key on [WhitelistId] in table 'Whitelists'
ALTER TABLE [dbo].[Whitelists]
ADD CONSTRAINT [PK_Whitelists]
    PRIMARY KEY CLUSTERED ([WhitelistId] ASC);
GO

-- Creating primary key on [WhitelistTypeId] in table 'WhitelistTypes'
ALTER TABLE [dbo].[WhitelistTypes]
ADD CONSTRAINT [PK_WhitelistTypes]
    PRIMARY KEY CLUSTERED ([WhitelistTypeId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AccountId] in table 'AccountGroups'
ALTER TABLE [dbo].[AccountGroups]
ADD CONSTRAINT [FK_AccountAccountGroups]
    FOREIGN KEY ([AccountId])
    REFERENCES [dbo].[Accounts]
        ([AccountId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountAccountGroups'
CREATE INDEX [IX_FK_AccountAccountGroups]
ON [dbo].[AccountGroups]
    ([AccountId]);
GO

-- Creating foreign key on [GroupId] in table 'AccountGroups'
ALTER TABLE [dbo].[AccountGroups]
ADD CONSTRAINT [FK_GroupsAccountGroups]
    FOREIGN KEY ([GroupId])
    REFERENCES [dbo].[Groups]
        ([GroupId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_GroupsAccountGroups'
CREATE INDEX [IX_FK_GroupsAccountGroups]
ON [dbo].[AccountGroups]
    ([GroupId]);
GO

-- Creating foreign key on [AccountId] in table 'AccountTickets'
ALTER TABLE [dbo].[AccountTickets]
ADD CONSTRAINT [FK_AccountAccountTicket]
    FOREIGN KEY ([AccountId])
    REFERENCES [dbo].[Accounts]
        ([AccountId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountAccountTicket'
CREATE INDEX [IX_FK_AccountAccountTicket]
ON [dbo].[AccountTickets]
    ([AccountId]);
GO

-- Creating foreign key on [AccountId] in table 'Detections'
ALTER TABLE [dbo].[Detections]
ADD CONSTRAINT [FK_AccountEvents]
    FOREIGN KEY ([AccountId])
    REFERENCES [dbo].[Accounts]
        ([AccountId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountEvents'
CREATE INDEX [IX_FK_AccountEvents]
ON [dbo].[Detections]
    ([AccountId]);
GO

-- Creating foreign key on [TicketId] in table 'AccountTickets'
ALTER TABLE [dbo].[AccountTickets]
ADD CONSTRAINT [FK_TicketAccountTicket]
    FOREIGN KEY ([TicketId])
    REFERENCES [dbo].[Tickets]
        ([TicketId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TicketAccountTicket'
CREATE INDEX [IX_FK_TicketAccountTicket]
ON [dbo].[AccountTickets]
    ([TicketId]);
GO

-- Creating foreign key on [CustomerId] in table 'AuthenticationLogs'
ALTER TABLE [dbo].[AuthenticationLogs]
ADD CONSTRAINT [FK_CustomerAuthenticationLogs]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([CustomerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerAuthenticationLogs'
CREATE INDEX [IX_FK_CustomerAuthenticationLogs]
ON [dbo].[AuthenticationLogs]
    ([CustomerId]);
GO

-- Creating foreign key on [CustomerId] in table 'CustomerIOCEvents'
ALTER TABLE [dbo].[CustomerIOCEvents]
ADD CONSTRAINT [FK_CustomerCustomerIOCEvent]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([CustomerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerCustomerIOCEvent'
CREATE INDEX [IX_FK_CustomerCustomerIOCEvent]
ON [dbo].[CustomerIOCEvents]
    ([CustomerId]);
GO

-- Creating foreign key on [DetectionDeviceId] in table 'CustomerIOCEvents'
ALTER TABLE [dbo].[CustomerIOCEvents]
ADD CONSTRAINT [FK_DetectionDeviceCustomerIOCEvent]
    FOREIGN KEY ([DetectionDeviceId])
    REFERENCES [dbo].[DetectionDevices]
        ([DetectionDeviceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetectionDeviceCustomerIOCEvent'
CREATE INDEX [IX_FK_DetectionDeviceCustomerIOCEvent]
ON [dbo].[CustomerIOCEvents]
    ([DetectionDeviceId]);
GO

-- Creating foreign key on [CustomerId] in table 'Detections'
ALTER TABLE [dbo].[Detections]
ADD CONSTRAINT [FK_CustomerDetection]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([CustomerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerDetection'
CREATE INDEX [IX_FK_CustomerDetection]
ON [dbo].[Detections]
    ([CustomerId]);
GO

-- Creating foreign key on [CustomerId] in table 'UserActivityLogs'
ALTER TABLE [dbo].[UserActivityLogs]
ADD CONSTRAINT [FK_CustomerDetectStatusLogs]
    FOREIGN KEY ([CustomerId])
    REFERENCES [dbo].[Customers]
        ([CustomerId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_CustomerDetectStatusLogs'
CREATE INDEX [IX_FK_CustomerDetectStatusLogs]
ON [dbo].[UserActivityLogs]
    ([CustomerId]);
GO

-- Creating foreign key on [DetectionDeviceId] in table 'Detections'
ALTER TABLE [dbo].[Detections]
ADD CONSTRAINT [FK_DetectionDeviceDetection]
    FOREIGN KEY ([DetectionDeviceId])
    REFERENCES [dbo].[DetectionDevices]
        ([DetectionDeviceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetectionDeviceDetection'
CREATE INDEX [IX_FK_DetectionDeviceDetection]
ON [dbo].[Detections]
    ([DetectionDeviceId]);
GO

-- Creating foreign key on [DeviceId] in table 'DetectionDevices'
ALTER TABLE [dbo].[DetectionDevices]
ADD CONSTRAINT [FK_DeviceDetectionDevice]
    FOREIGN KEY ([DeviceId])
    REFERENCES [dbo].[Devices]
        ([DeviceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DeviceDetectionDevice'
CREATE INDEX [IX_FK_DeviceDetectionDevice]
ON [dbo].[DetectionDevices]
    ([DeviceId]);
GO

-- Creating foreign key on [DetectionId] in table 'DetectionTaxonomies'
ALTER TABLE [dbo].[DetectionTaxonomies]
ADD CONSTRAINT [FK_DetectionDetectionTaxonomies]
    FOREIGN KEY ([DetectionId])
    REFERENCES [dbo].[Detections]
        ([DetectionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetectionDetectionTaxonomies'
CREATE INDEX [IX_FK_DetectionDetectionTaxonomies]
ON [dbo].[DetectionTaxonomies]
    ([DetectionId]);
GO

-- Creating foreign key on [DetectionId] in table 'DnsRequests'
ALTER TABLE [dbo].[DnsRequests]
ADD CONSTRAINT [FK_DetectionDnsRequests]
    FOREIGN KEY ([DetectionId])
    REFERENCES [dbo].[Detections]
        ([DetectionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetectionDnsRequests'
CREATE INDEX [IX_FK_DetectionDnsRequests]
ON [dbo].[DnsRequests]
    ([DetectionId]);
GO

-- Creating foreign key on [DetectionId] in table 'DocumentsAccesses'
ALTER TABLE [dbo].[DocumentsAccesses]
ADD CONSTRAINT [FK_DetectionDocumentsAccessed]
    FOREIGN KEY ([DetectionId])
    REFERENCES [dbo].[Detections]
        ([DetectionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetectionDocumentsAccessed'
CREATE INDEX [IX_FK_DetectionDocumentsAccessed]
ON [dbo].[DocumentsAccesses]
    ([DetectionId]);
GO

-- Creating foreign key on [DetectionId] in table 'DetectionTags'
ALTER TABLE [dbo].[DetectionTags]
ADD CONSTRAINT [FK_DetectionEventTags]
    FOREIGN KEY ([DetectionId])
    REFERENCES [dbo].[Detections]
        ([DetectionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetectionEventTags'
CREATE INDEX [IX_FK_DetectionEventTags]
ON [dbo].[DetectionTags]
    ([DetectionId]);
GO

-- Creating foreign key on [DetectionId] in table 'ExecutablesWrittens'
ALTER TABLE [dbo].[ExecutablesWrittens]
ADD CONSTRAINT [FK_DetectionExecutablesWritten]
    FOREIGN KEY ([DetectionId])
    REFERENCES [dbo].[Detections]
        ([DetectionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetectionExecutablesWritten'
CREATE INDEX [IX_FK_DetectionExecutablesWritten]
ON [dbo].[ExecutablesWrittens]
    ([DetectionId]);
GO

-- Creating foreign key on [DetectionId] in table 'NetworkAccesses'
ALTER TABLE [dbo].[NetworkAccesses]
ADD CONSTRAINT [FK_DetectionNetworkAccesses]
    FOREIGN KEY ([DetectionId])
    REFERENCES [dbo].[Detections]
        ([DetectionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetectionNetworkAccesses'
CREATE INDEX [IX_FK_DetectionNetworkAccesses]
ON [dbo].[NetworkAccesses]
    ([DetectionId]);
GO

-- Creating foreign key on [DetectionId] in table 'ScanResults'
ALTER TABLE [dbo].[ScanResults]
ADD CONSTRAINT [FK_DetectionScanResults]
    FOREIGN KEY ([DetectionId])
    REFERENCES [dbo].[Detections]
        ([DetectionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetectionScanResults'
CREATE INDEX [IX_FK_DetectionScanResults]
ON [dbo].[ScanResults]
    ([DetectionId]);
GO

-- Creating foreign key on [DetectionId] in table 'DetectionTickets'
ALTER TABLE [dbo].[DetectionTickets]
ADD CONSTRAINT [FK_DetectionTicketEvent]
    FOREIGN KEY ([DetectionId])
    REFERENCES [dbo].[Detections]
        ([DetectionId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DetectionTicketEvent'
CREATE INDEX [IX_FK_DetectionTicketEvent]
ON [dbo].[DetectionTickets]
    ([DetectionId]);
GO

-- Creating foreign key on [ResponderId] in table 'Detections'
ALTER TABLE [dbo].[Detections]
ADD CONSTRAINT [FK_ResponderDetection]
    FOREIGN KEY ([ResponderId])
    REFERENCES [dbo].[Responders]
        ([ResponderId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ResponderDetection'
CREATE INDEX [IX_FK_ResponderDetection]
ON [dbo].[Detections]
    ([ResponderId]);
GO

-- Creating foreign key on [CustomSeverityId] in table 'Detections'
ALTER TABLE [dbo].[Detections]
ADD CONSTRAINT [FK_SeverityDetection]
    FOREIGN KEY ([CustomSeverityId])
    REFERENCES [dbo].[Severities]
        ([SeverityId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SeverityDetection'
CREATE INDEX [IX_FK_SeverityDetection]
ON [dbo].[Detections]
    ([CustomSeverityId]);
GO

-- Creating foreign key on [StatusId] in table 'Detections'
ALTER TABLE [dbo].[Detections]
ADD CONSTRAINT [FK_StatusDetection]
    FOREIGN KEY ([StatusId])
    REFERENCES [dbo].[Status]
        ([StatusId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_StatusDetection'
CREATE INDEX [IX_FK_StatusDetection]
ON [dbo].[Detections]
    ([StatusId]);
GO

-- Creating foreign key on [TagId] in table 'DetectionTags'
ALTER TABLE [dbo].[DetectionTags]
ADD CONSTRAINT [FK_TagEventTags]
    FOREIGN KEY ([TagId])
    REFERENCES [dbo].[Tags]
        ([TagId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TagEventTags'
CREATE INDEX [IX_FK_TagEventTags]
ON [dbo].[DetectionTags]
    ([TagId]);
GO

-- Creating foreign key on [TaxonomyId] in table 'DetectionTaxonomies'
ALTER TABLE [dbo].[DetectionTaxonomies]
ADD CONSTRAINT [FK_TaxonomyDetectionTaxonomies]
    FOREIGN KEY ([TaxonomyId])
    REFERENCES [dbo].[Taxonomies]
        ([TaxonomyId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaxonomyDetectionTaxonomies'
CREATE INDEX [IX_FK_TaxonomyDetectionTaxonomies]
ON [dbo].[DetectionTaxonomies]
    ([TaxonomyId]);
GO

-- Creating foreign key on [TicketId] in table 'DetectionTickets'
ALTER TABLE [dbo].[DetectionTickets]
ADD CONSTRAINT [FK_TicketDetectionTicket]
    FOREIGN KEY ([TicketId])
    REFERENCES [dbo].[Tickets]
        ([TicketId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TicketDetectionTicket'
CREATE INDEX [IX_FK_TicketDetectionTicket]
ON [dbo].[DetectionTickets]
    ([TicketId]);
GO

-- Creating foreign key on [DeviceId] in table 'DeviceTickets'
ALTER TABLE [dbo].[DeviceTickets]
ADD CONSTRAINT [FK_DeviceDeviceTicket]
    FOREIGN KEY ([DeviceId])
    REFERENCES [dbo].[Devices]
        ([DeviceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_DeviceDeviceTicket'
CREATE INDEX [IX_FK_DeviceDeviceTicket]
ON [dbo].[DeviceTickets]
    ([DeviceId]);
GO

-- Creating foreign key on [TicketId] in table 'DeviceTickets'
ALTER TABLE [dbo].[DeviceTickets]
ADD CONSTRAINT [FK_TicketDeviceTicket]
    FOREIGN KEY ([TicketId])
    REFERENCES [dbo].[Tickets]
        ([TicketId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TicketDeviceTicket'
CREATE INDEX [IX_FK_TicketDeviceTicket]
ON [dbo].[DeviceTickets]
    ([TicketId]);
GO

-- Creating foreign key on [ResponderId] in table 'ResponderSchedules'
ALTER TABLE [dbo].[ResponderSchedules]
ADD CONSTRAINT [FK_ResponderResponderSchedule]
    FOREIGN KEY ([ResponderId])
    REFERENCES [dbo].[Responders]
        ([ResponderId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ResponderResponderSchedule'
CREATE INDEX [IX_FK_ResponderResponderSchedule]
ON [dbo].[ResponderSchedules]
    ([ResponderId]);
GO

-- Creating foreign key on [SeverityId] in table 'Tickets'
ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_SeverityTicket]
    FOREIGN KEY ([SeverityId])
    REFERENCES [dbo].[Severities]
        ([SeverityId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_SeverityTicket'
CREATE INDEX [IX_FK_SeverityTicket]
ON [dbo].[Tickets]
    ([SeverityId]);
GO

-- Creating foreign key on [TaxTypeId] in table 'Taxonomies'
ALTER TABLE [dbo].[Taxonomies]
ADD CONSTRAINT [FK_TaxonomyTypesTaxonomy]
    FOREIGN KEY ([TaxTypeId])
    REFERENCES [dbo].[TaxonomyTypes]
        ([TaxTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TaxonomyTypesTaxonomy'
CREATE INDEX [IX_FK_TaxonomyTypesTaxonomy]
ON [dbo].[Taxonomies]
    ([TaxTypeId]);
GO

-- Creating foreign key on [TicketRecipientId] in table 'Tickets'
ALTER TABLE [dbo].[Tickets]
ADD CONSTRAINT [FK_TicketRecipientTicket]
    FOREIGN KEY ([TicketRecipientId])
    REFERENCES [dbo].[TicketRecipients]
        ([TicketRecipientId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_TicketRecipientTicket'
CREATE INDEX [IX_FK_TicketRecipientTicket]
ON [dbo].[Tickets]
    ([TicketRecipientId]);
GO

-- Creating foreign key on [WhitelistTypeId] in table 'Whitelists'
ALTER TABLE [dbo].[Whitelists]
ADD CONSTRAINT [FK_WhitelistTypeWhitelist]
    FOREIGN KEY ([WhitelistTypeId])
    REFERENCES [dbo].[WhitelistTypes]
        ([WhitelistTypeId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_WhitelistTypeWhitelist'
CREATE INDEX [IX_FK_WhitelistTypeWhitelist]
ON [dbo].[Whitelists]
    ([WhitelistTypeId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------