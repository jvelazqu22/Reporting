USE [iBankAdministration]
GO
delete [dbo].[ibank_feature_flag] where toggle_name = 'UdidUserDefined'
