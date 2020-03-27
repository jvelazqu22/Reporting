Feature: PendingReportsRetriever
	In order to process certain agencies and databases on different report server versions
	As a systems user
	I want to filter the retrieved pending reports to process based on report server type

Scenario: Primary Server in Dev Mode Agency On Stage No Database On Stage Return All Pending Reports
Given the report server is functioning as 'Primary'
	And the server 'is' in DevMode
	And a report for agency 'STAGED' with id 'STAGED_1' 'is' pending
	And a report for agency 'STAGED' with id 'STAGED_2' 'is not' pending
	And a report for agency 'NOTSTAGED' with id 'NOTSTAGED_1' 'is' pending
	And a report for agency 'NOTSTAGED' with id 'NOTSTAGED_2' 'is not' pending
	And the agency 'STAGED' is on stage
	And the agency 'STAGED' belongs to database 'S_DATABASE'
	And the agency 'NOTSTAGED' belongs to database 'NS_DATABASE'
	And the pending reports table has a report id 'STAGED_1'
	And the pending reports table has a report id 'NOTSTAGED_1'
When I retrieve the reports
Then I have a total of '2' reports
	And the reportid 'STAGED_1' exists
	And the reportid 'NOTSTAGED_1' exists

Scenario: Primary Server not in Dev Mode Agency On Stage No Database On Stage Dont Return Staged Agency Reports
Given the report server is functioning as 'Primary'
	And the server 'is not' in DevMode
	And a report for agency 'STAGED' with id 'STAGED_1' 'is' pending
	And a report for agency 'STAGED' with id 'STAGED_2' 'is not' pending
	And a report for agency 'NOTSTAGED' with id 'NOTSTAGED_1' 'is' pending
	And a report for agency 'NOTSTAGED' with id 'NOTSTAGED_2' 'is not' pending
	And the agency 'STAGED' is on stage
	And the agency 'STAGED' belongs to database 'S_DATABASE'
	And the agency 'NOTSTAGED' belongs to database 'NS_DATABASE'
	And the pending reports table has a report id 'NOTSTAGED_1'
When I retrieve the reports
Then I have a total of '1' reports
	And the reportid 'NOTSTAGED_1' exists

Scenario: Primary Server not in Dev Mode No Agency On Stage Database On Stage Dont Return Staged Agency Reports
Given the report server is functioning as 'Primary'
	And the server 'is not' in DevMode
	And a report for agency 'STAGED' with id 'STAGED_1' 'is' pending
	And a report for agency 'STAGED' with id 'STAGED_2' 'is not' pending
	And a report for agency 'NOTSTAGED' with id 'NOTSTAGED_1' 'is' pending
	And a report for agency 'NOTSTAGED' with id 'NOTSTAGED_2' 'is not' pending
	And the database 'S_DATABASE' is on stage
	And the agency 'STAGED' belongs to database 'S_DATABASE'
	And the agency 'NOTSTAGED' belongs to database 'NS_DATABASE'
	And the pending reports table has a report id 'NOTSTAGED_1'
When I retrieve the reports
Then I have a total of '1' reports
	And the reportid 'NOTSTAGED_1' exists

Scenario: Stage Server not in Dev Mode Agency On Stage No Database On Stage Return Staged Agency Reports
Given the report server is functioning as 'Stage'
	And the server 'is not' in DevMode
	And a report for agency 'STAGED' with id 'STAGED_1' 'is' pending
	And a report for agency 'STAGED' with id 'STAGED_2' 'is not' pending
	And a report for agency 'NOTSTAGED' with id 'NOTSTAGED_1' 'is' pending
	And a report for agency 'NOTSTAGED' with id 'NOTSTAGED_2' 'is not' pending
	And the agency 'STAGED' is on stage
	And the agency 'STAGED' belongs to database 'S_DATABASE'
	And the agency 'NOTSTAGED' belongs to database 'NS_DATABASE'
	And the pending reports table has a report id 'STAGED_1'
When I retrieve the reports
Then I have a total of '1' reports
	And the reportid 'STAGED_1' exists

Scenario: Stage Server not in Dev Mode No Agency On Stage Database On Stage Return Staged Agency Reports
Given the report server is functioning as 'Stage'
	And the server 'is not' in DevMode
	And a report for agency 'STAGED' with id 'STAGED_1' 'is' pending
	And a report for agency 'STAGED' with id 'STAGED_2' 'is not' pending
	And a report for agency 'NOTSTAGED' with id 'NOTSTAGED_1' 'is' pending
	And a report for agency 'NOTSTAGED' with id 'NOTSTAGED_2' 'is not' pending
	And the database 'S_DATABASE' is on stage
	And the agency 'STAGED' belongs to database 'S_DATABASE'
	And the agency 'NOTSTAGED' belongs to database 'NS_DATABASE'
	And the pending reports table has a report id 'STAGED_1'
When I retrieve the reports
Then I have a total of '1' reports
	And the reportid 'STAGED_1' exists

