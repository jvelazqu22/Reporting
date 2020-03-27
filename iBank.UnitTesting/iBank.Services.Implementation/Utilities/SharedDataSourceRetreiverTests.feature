Feature: SharedDataSourceRetreiverTests
	In order to view data for my agencies
	As a shared data user 
	I need to get all the server and database pairs for my agencies

Scenario: Agencies are spread across multiple servers and databases
	Given I am searching under master corp account 'Foo'
		And 'FooChild' is an agency under 'Foo' can access acct 'FooAcct' and sourceabbr 'FooSource'
		And 'FooChild' is located on database 'FooDatabase'
		And 'FooDatabase' is located on server 'FooServer'
		And 'BarChild' is an agency under 'Foo' can access acct 'BarAcct' and sourceabbr 'BarSource'
		And 'BarChild' is located on database 'BarDatabase'
		And 'BarDatabase' is located on server 'BarServer'
	When I get all the datasources
	Then I have '2' data sources
		And the data source with database 'FooDatabase' is on server 'FooServer' and agency 'FooChild'
		And the data source with database 'BarDatabase' is on server 'BarServer' and agency 'BarChild'


Scenario: Agencies are all on the same server and database
	Given I am searching under master corp account 'Foo'
		And 'FooChild' is an agency under 'Foo' can access acct 'FooAcct' and sourceabbr 'FooSource'
		And 'FooChild' is located on database 'Database'
		And 'BarChild' is an agency under 'Foo' can access acct 'BarAcct' and sourceabbr 'BarSource'
		And 'BarChild' is located on database 'Database'
		And 'Database' is located on server 'Server'
	When I get all the datasources
	Then I have '2' data sources
		And the data source with database 'Database' is on server 'Server' and agency 'FooChild'
		And the data source with database 'Database' is on server 'Server' and agency 'BarChild'

