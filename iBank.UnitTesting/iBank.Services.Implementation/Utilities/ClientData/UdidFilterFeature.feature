Feature: UdidFilterFeature
	In order to filter records
	As a user
	I need to filter my data based on UDID criteria

# Scenarios
	# I. And Join - all the filters criteria must be met to retain record
	# II. Or Join - at least one of the filters criteria must be met to retain record

	# 1. No udids for reckey
		# a. Not Equal - retain
		# b. Equal - discard
	# 2. Udids for reckey, udid filter number doesn't match
		# a. Not Equal - retain
		# b. Equal - discard
	# 3. Udids for reckey, udid filter number matches, udid filter text does not match
		# a. Not Equal - retain
		# b. Equal - discard
	# 4. Udids for reckey, udid filter number matches, udid filter text matches
		# a. Not Equal - discard
		# b. Equal - retain

	# Operator Types
		# a. All Equal - each udid filter has an Equal operator
		# b. All Not Equal - each udid filter has a Not Equal operator
		# c. Mixed Equal And Not Equal - at least one filter has an Equal operator, at least one filter has a Not Equal operator
#

# Start - No udids for reckey

Scenario: And Join All Equal Operators No Udids For Reckey Discard Record
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'Equal'
		And I have a join type of 'And'
		And I have a udid record with a reckey '99' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '99' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: And Join All Not Equal Operators No Udids For Reckey Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'NotEqual'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'And'
		And I have a udid record with a reckey '99' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '99' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does exist

Scenario: And Join Mixed Equal Not Equal Operators No Udids For Reckey Discard
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'And'
		And I have a udid record with a reckey '99' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '99' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: Or Join All Equal Operators No Udids For Reckey Discard Record
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'Equal'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '99' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '99' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: Or Join All Not Equal Operators No Udids For Reckey Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'NotEqual'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '99' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '99' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does exist

Scenario: Or Join Mixed Equal Not Equal Operators No Udids For Reckey Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '99' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '99' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does exist

# End - No udids for reckey

# Start - Udids for reckey, udid filter number doesn't match

Scenario: And Join All Equal Operators Udids For Reckey Udid Filter Number Does Not Match Discard
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'Equal'
		And I have a join type of 'And'
		And I have a udid record with a reckey '1' and udid number of '99' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '100' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: And Join All Not Equal Operators Udids For Reckey Udid Filter Number Does Not Match Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'NotEqual'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'And'
		And I have a udid record with a reckey '1' and udid number of '99' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '100' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does exist

Scenario: And Join Mixed Equal Not Equal Operators Udids For Reckey Udid Filter Number Does Not Match Discard
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'And'
		And I have a udid record with a reckey '1' and udid number of '99' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '100' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: Or Join All Equal Operators Udids For Reckey Udid Filter Number Does Not Match Discard
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'Equal'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '1' and udid number of '99' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '100' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: Or Join All Not Equal Operators Udids For Reckey Udid Filter Number Does Not Match Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'NotEqual'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '1' and udid number of '99' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '100' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does exist

Scenario: Or Join Mixed Equal Not Equal Operators Udids For Reckey Udid Filter Number Does Not Match Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '1' and udid number of '99' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '100' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does exist

# End - Udids for reckey, udid filter number doesn't match

# Start - Udids for reckey, udid filter number matches, udid filter text does not match

Scenario: And Join All Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Not Match Discard
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'Equal'
		And I have a join type of 'And'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'blah'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'blah'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: And Join All Not Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Not Match Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'NotEqual'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'And'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'blah'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'blah'
	When I filter the data
	Then the reckey '1' does exist

Scenario: And Join Mixed Equal Not Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Not Match Discard
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'And'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'blah'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'blah'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: Or Join All Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Not Match Discard
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'Equal'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'blah'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'blah'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: Or Join All Not Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Not Match Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'NotEqual'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'blah'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'blah'
	When I filter the data
	Then the reckey '1' does exist

Scenario: Or Join Mixed Equal Not Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Not Match Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'blah'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'blah'
	When I filter the data
	Then the reckey '1' does exist

# End - Udids for reckey, udid filter number matches, udid filter text does not match

# Start - Udids for reckey, udid filter number matches, udid filter text matches

Scenario: And Join All Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Match Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'Equal'
		And I have a join type of 'And'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does exist

Scenario: And Join All Not Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Match Discard
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'NotEqual'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'And'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: And Join Mixed Equal Not Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Match Discard
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'And'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: Or Join All Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Match Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'Equal'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does exist

Scenario: Or Join All Not Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Match Discard
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'NotEqual'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does not exist

Scenario: Or Join Mixed Equal Not Equal Operators Udids For Reckey Udid Filter Number Does Match Udid Filter Text Does Match Retain
	Given I have a data record with a reckey of '1'
		And I have a udid filter with a udid number of 10 and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid filter with a udid number of 11 and a udid text of 'bar' and an operator of 'NotEqual'
		And I have a join type of 'Or'
		And I have a udid record with a reckey '1' and udid number of '10' and udid text of 'foo'
		And I have a udid record with a reckey '1' and udid number of '11' and udid text of 'bar'
	When I filter the data
	Then the reckey '1' does exist

# End - Udids for reckey, udid filter number matches, udid filter text matches




