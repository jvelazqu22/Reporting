Feature: MultiUdidConditionalFeature
	In order to filter records with an advanced udid criteria
	I need to determine if a record should be kept or discarded
	By comparing the udid number and udid text


Scenario: MultipleUdidFilters_EqualsOperator_AndJoin_KeepAllRecordsWithSameUdidNumberAndSameUdidText
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '1' and a udid number of '11' and a udid text of 'foobar'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '11' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '12' and a udid text of 'foobar'
		And I have a udid parameter with a udid number of '10' and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foobar' and an operator of 'Equal'
		And I have an AndOr of 'And'
	When I get the reckeys to keep
	Then I have '1' distinct reckeys remaining
		And the reckey '1' still exists
		And the reckey '2' no longer exists
		And the reckey '3' no longer exists

Scenario: MultipleUdidFilters_EqualsOperator_AndJoin_HasWildcards_KeepAllRecordsWithSameUdidNumberAndSameUdidText
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '1' and a udid number of '11' and a udid text of 'foobar'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '11' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '12' and a udid text of 'foobar'
		And I have a udid parameter with a udid number of '10' and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foob%' and an operator of 'Equal'
		And I have an AndOr of 'And'
	When I get the reckeys to keep
	Then I have '1' distinct reckeys remaining
		And the reckey '1' still exists
		And the reckey '2' no longer exists
		And the reckey '3' no longer exists

Scenario: MultipleUdidFilters Not Equals Operator Or Join One of the Udids Doesnt Exist Return Reckey
	Given I have a record with a reckey of '585650586' and a udid number of '800' and a udid text of '379630XXXXX1005'
		And I have a udid parameter with a udid number of '805' and a udid text of '%' and an operator of 'NotEqual'
		And I have a udid parameter with a udid number of '800' and a udid text of '%' and an operator of 'NotEqual'
	When I get the reckeys to keep
	Then I have '1' distinct reckeys remaining
		And the reckey '585650586' still exists

Scenario: MultipleUdidFilters_EqualsOperator_OrJoin_KeepAllRecordsWithAtLeastOneMatchingUdidNumberAndUdidText
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '1' and a udid number of '11' and a udid text of 'foobar'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '11' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '12' and a udid text of 'foobar'
		And I have a record with a reckey of '4' and a udid number of '10' and a udid text of 'blah'
		And I have a record with a reckey of '4' and a udid number of '11' and a udid text of 'blah blah'
		And I have a udid parameter with a udid number of '10' and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foobar' and an operator of 'Equal'
		And I have an AndOr of 'Or'
	When I get the reckeys to keep
	Then I have '2' distinct reckeys remaining
		And the reckey '1' still exists
		And the reckey '2' still exists
		And the reckey '3' no longer exists
		And the reckey '4' no longer exists

Scenario: MultipleUdidFilters_EqualsOperator_OrJoin_HasWildcards_KeepAllRecordsWithAtLeastOneMatchingUdidNumberAndUdidText
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '1' and a udid number of '11' and a udid text of 'foobar'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '11' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '12' and a udid text of 'foobar'
		And I have a record with a reckey of '4' and a udid number of '10' and a udid text of 'blah'
		And I have a record with a reckey of '4' and a udid number of '11' and a udid text of 'blah blah'
		And I have a udid parameter with a udid number of '10' and a udid text of 'f%' and an operator of 'Equal'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foobar' and an operator of 'Equal'
		And I have an AndOr of 'Or'
	When I get the reckeys to keep
	Then I have '2' distinct reckeys remaining
		And the reckey '1' still exists
		And the reckey '2' still exists
		And the reckey '3' no longer exists
		And the reckey '4' no longer exists

Scenario: MultipleUdidFilters_NotEqualsOperator_AndJoin_KeepAllRecordsWithDifferentUdidNumber_KeepAllRecordsWithSameUdidNumberButDifferentText
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '1' and a udid number of '11' and a udid text of 'foobar'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '11' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '12' and a udid text of 'foobar'
		And I have a record with a reckey of '4' and a udid number of '10' and a udid text of 'blah'
		And I have a record with a reckey of '4' and a udid number of '11' and a udid text of 'blah blah'
		And I have a udid parameter with a udid number of '10' and a udid text of 'foo' and an operator of 'NotEqual'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foobar' and an operator of 'NotEqual'
		And I have an AndOr of 'And'
	When I get the reckeys to keep
	Then I have '2' distinct reckeys remaining
		And the reckey '1' no longer exists
		And the reckey '2' no longer exists
		And the reckey '3' still exists
		And the reckey '4' still exists

Scenario: SingleUdidFilter_NotEqualsOperator_AndJoin_KeepAllRecordsWithDifferentUdidNumber_KeepAllRecordsWithSameUdidNumberButDifferentText
	Given I have a record with a reckey of '1' and a udid number of '11' and a udid text of 'bar'
		And I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo' 
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '3' and a udid number of '10' and a udid text of 'bar'
		And I have a record with a reckey of '4' and a udid number of '11' and a udid text of 'blah'
		And I have a udid parameter with a udid number of '10' and a udid text of 'foo' and an operator of 'NotEqual'
		And I have an AndOr of 'And'
	When I get the reckeys to keep
	Then I have '2' distinct reckeys remaining
		And the reckey '1' no longer exists
		And the reckey '2' no longer exists
		And the reckey '3' still exists
		And the reckey '4' still exists

Scenario: MultipleUdidFilters_NotEqualsOperator_AndJoin_HasWildcards_KeepAllRecordsWithDifferentUdidNumber_KeepAllRecordsWithSameUdidNumberButDifferentText
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'bar'
		And I have a record with a reckey of '1' and a udid number of '11' and a udid text of 'foobar'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '11' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '12' and a udid text of 'foobar'
		And I have a record with a reckey of '4' and a udid number of '10' and a udid text of 'blah'
		And I have a record with a reckey of '4' and a udid number of '11' and a udid text of 'blah blah'
		And I have a udid parameter with a udid number of '10' and a udid text of 'foo' and an operator of 'NotEqual'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foob%' and an operator of 'NotEqual'
		And I have an AndOr of 'And'
	When I get the reckeys to keep
	Then I have '2' distinct reckeys remaining
		And the reckey '1' no longer exists
		And the reckey '2' no longer exists
		And the reckey '3' still exists
		And the reckey '4' still exists

Scenario: MultipleUdidFilters_NotEqualsOperator_OrJoin_KeepAllRecordsWithDifferentUdidNumber_KeepAllRecordsWithAtLeastOneSameUdidNumberButDifferentText
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '1' and a udid number of '11' and a udid text of 'foobar'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '11' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '12' and a udid text of 'foobar'
		And I have a record with a reckey of '4' and a udid number of '10' and a udid text of 'bar'
		And I have a record with a reckey of '4' and a udid number of '11' and a udid text of 'foobar'
		And I have a udid parameter with a udid number of '10' and a udid text of 'foo' and an operator of 'NotEqual'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foobar' and an operator of 'NotEqual'
		And I have an AndOr of 'Or'
	When I get the reckeys to keep
	Then I have '2' distinct reckeys remaining
		And the reckey '1' no longer exists
		And the reckey '2' no longer exists
		And the reckey '3' still exists
		And the reckey '4' still exists

Scenario: MultipleUdidFilters_NotEqualsOperator_OrJoin_HasWildcards_KeepAllRecordsWithDifferentUdidNumber_KeepAllRecordsWithAtLeastOneSameUdidNumberButDifferentText
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '1' and a udid number of '11' and a udid text of 'foobar'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '11' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '12' and a udid text of 'foobar'
		And I have a record with a reckey of '4' and a udid number of '10' and a udid text of 'bar'
		And I have a record with a reckey of '4' and a udid number of '11' and a udid text of 'foobar'
		And I have a udid parameter with a udid number of '10' and a udid text of 'f%' and an operator of 'NotEqual'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foobar' and an operator of 'NotEqual'
		And I have an AndOr of 'Or'
	When I get the reckeys to keep
	Then I have '2' distinct reckeys remaining
		And the reckey '1' no longer exists
		And the reckey '2' no longer exists
		And the reckey '3' still exists
		And the reckey '4' still exists

Scenario: MultipleUdidFilters_MixedEqualsAndNotEqualsOperator_AndJoin_KeepRecordsWithSameUdidNumberAndTextAsEqualFilter_And_NoOtherUdidNumber_OrDifferentUdidNumberThanNotEqualFilter_OrSameUdidNumberButDifferentUdidTextAsNotEqualFilter
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '12' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '3' and a udid number of '11' and a udid text of 'blah'
		And I have a record with a reckey of '4' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '4' and a udid number of '11' and a udid text of 'foobar'
		And I have a udid parameter with a udid number of '10' and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foobar' and an operator of 'NotEqual'
		And I have an AndOr of 'And'
	When I get the reckeys to keep
	Then I have '3' distinct reckeys remaining
		And the reckey '1' still exists
		And the reckey '2' still exists
		And the reckey '3' still exists
		And the reckey '4' no longer exists

Scenario: MultipleUdidFilters_MixedEqualsAndNotEqualsOperator_AndJoin_HasWildcards_KeepRecordsWithSameUdidNumberAndTextAsEqualFilter_And_NoOtherUdidNumber_OrDifferentUdidNumberThanNotEqualFilter_OrSameUdidNumberButDifferentUdidTextAsNotEqualFilter
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '2' and a udid number of '12' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '3' and a udid number of '11' and a udid text of 'blah'
		And I have a record with a reckey of '4' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '4' and a udid number of '11' and a udid text of 'fooblast'
		And I have a udid parameter with a udid number of '10' and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foob%' and an operator of 'NotEqual'
		And I have an AndOr of 'And'
	When I get the reckeys to keep
	Then I have '3' distinct reckeys remaining
		And the reckey '1' still exists
		And the reckey '2' still exists
		And the reckey '3' still exists
		And the reckey '4' no longer exists

Scenario: MultipleUdidFilters_MixedEqualsAndNotEqualsOperator_OrJoin_KeepRecordsWithAtLeast_UdidTextAndUdidNumberEqualOnEqualFilter_UdidNumberEqualButUdidTextDifferentOnNotEqualFilter_OrAUdidNumberNotInFilter
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '1' and a udid number of '11' and a udid text of 'bar'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'bar'
		And I have a record with a reckey of '2' and a udid number of '11' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '3' and a udid number of '12' and a udid text of 'blah'
		And I have a record with a reckey of '4' and a udid number of '10' and a udid text of 'bar'
		And I have a record with a reckey of '4' and a udid number of '11' and a udid text of 'foobar'
		And I have a record with a reckey of '5' and a udid number of '20' and a udid text of 'blah'
		And I have a udid parameter with a udid number of '10' and a udid text of 'foo' and an operator of 'Equal'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foobar' and an operator of 'NotEqual'
		And I have an AndOr of 'Or'
	When I get the reckeys to keep
	Then I have '4' distinct reckeys remaining
		And the reckey '1' still exists
		And the reckey '2' still exists
		And the reckey '3' still exists
		And the reckey '4' no longer exists
		And the reckey '5' still exists

Scenario: MultipleUdidFilters_MixedEqualsAndNotEqualsOperator_OrJoin_HasWildcards_KeepRecordsWithAtLeast_UdidTextAndUdidNumberEqualOnEqualFilter_UdidNumberEqualButUdidTextDifferentOnNotEqualFilter_OrAUdidNumberNotInFilter
	Given I have a record with a reckey of '1' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '1' and a udid number of '11' and a udid text of 'bar'
		And I have a record with a reckey of '2' and a udid number of '10' and a udid text of 'bar'
		And I have a record with a reckey of '2' and a udid number of '11' and a udid text of 'nope'
		And I have a record with a reckey of '3' and a udid number of '10' and a udid text of 'foo'
		And I have a record with a reckey of '3' and a udid number of '12' and a udid text of 'blah'
		And I have a record with a reckey of '4' and a udid number of '10' and a udid text of 'bar'
		And I have a record with a reckey of '4' and a udid number of '11' and a udid text of 'foobar'
		And I have a record with a reckey of '5' and a udid number of '20' and a udid text of 'blah'
		And I have a udid parameter with a udid number of '10' and a udid text of 'f%' and an operator of 'Equal'
		And I have a udid parameter with a udid number of '11' and a udid text of 'foobar' and an operator of 'NotEqual'
		And I have an AndOr of 'Or'
	When I get the reckeys to keep
	Then I have '4' distinct reckeys remaining
		And the reckey '1' still exists
		And the reckey '2' still exists
		And the reckey '3' still exists
		And the reckey '4' no longer exists
		And the reckey '5' still exists

