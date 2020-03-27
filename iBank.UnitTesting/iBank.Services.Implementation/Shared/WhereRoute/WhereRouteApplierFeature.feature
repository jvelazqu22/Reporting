Feature: WhereRouteApplierFeature
	In order to retrieve subsets of data
	As a user
	I need to be able to filter a dataset based on leg and trip criteria

Scenario: GetDataBasedOnOriginCriteria_ReturnAllLegs_IsIn_LegWithinATripMatchesFilter_ReturnAllLegsOfThatTrip
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB' 
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '1' leg '3' has an origin of 'CCC' and a destination of 'DDD'
		And reckey '2' leg '1' has an origin of 'YYY' and a destination of 'XXX'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'ZZZ'
		And I have origin criteria of 'AAA'
		And I 'do' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on origin criteria
	Then I have '3' total records
		And reckey '1' exists
		And reckey '2' does not exist
		And a record with origin 'AAA' exists
		And a record with origin 'BBB' exists
		And a record with origin 'CCC' exists

Scenario: GetDataBasedOnOriginCriteria_ReturnAllLegs_NotIn_LegWithinTripMatchesFilter_DontReturnAnyLegsForThatTrip
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB' 
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '1' leg '3' has an origin of 'CCC' and a destination of 'DDD'
		And reckey '2' leg '1' has an origin of 'YYY' and a destination of 'XXX'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'ZZZ'
		And I have origin criteria of 'AAA'
		And I 'do' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin criteria
	Then I have '2' total records
		And reckey '1' does not exist
		And reckey '2' exists
		And a record with origin 'YYY' exists
		And a record with origin 'XXX' exists

Scenario: GetDataBasedOnOriginCriteria_DontReturnAllLegs_IsIn_LegWithinTripMatchesFilter_ReturnThatLeg
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB' 
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '1' leg '3' has an origin of 'CCC' and a destination of 'DDD'
		And reckey '2' leg '1' has an origin of 'YYY' and a destination of 'XXX'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'ZZZ'
		And I have origin criteria of 'AAA'
		And I 'do not' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on origin criteria
	Then I have '1' total records
		And reckey '1' exists
		And reckey '2' does not exist
		And a record with origin 'AAA' exists

Scenario: GetDataBasedOnOriginCriteria_DontReturnAllLegs_NotIn_ReturnAllLegsThatDoNotMatchFilter
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB' 
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '1' leg '3' has an origin of 'CCC' and a destination of 'DDD'
		And reckey '2' leg '1' has an origin of 'YYY' and a destination of 'XXX'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'ZZZ'
		And I have origin criteria of 'AAA'
		And I 'do not' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin criteria
	Then I have '4' total records
		And reckey '1' exists
		And reckey '2' exists
		And a record with origin 'BBB' exists
		And a record with origin 'CCC' exists
		And a record with origin 'YYY' exists
		And a record with origin 'XXX' exists

Scenario: GetDataBasedOnOriginCriteria__MultipleCriteria_ReturnAllLegs_IsIn_LegWithinATripMatchesFilter_ReturnAllLegsOfThatTrip
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB' 
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '1' leg '3' has an origin of 'CCC' and a destination of 'DDD'
		And reckey '2' leg '1' has an origin of 'YYY' and a destination of 'XXX'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'ZZZ'
		And reckey '3' leg '1' has an origin of 'EEE' and a destination of 'FFF'
		And I have origin criteria of 'AAA'
		And I have origin criteria of 'EEE'
		And I 'do' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on origin criteria
	Then I have '4' total records
		And reckey '1' exists
		And reckey '2' does not exist
		And reckey '3' exists
		And a record with origin 'AAA' exists
		And a record with origin 'BBB' exists
		And a record with origin 'CCC' exists
		And a record with origin 'EEE' exists

Scenario: GetDataBasedOnDestinationCriteria_ReturnAllLegs_IsIn_LegWithinATripMatchesFilter_ReturnAllLegsOfThatTrip
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB' 
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '1' leg '3' has an origin of 'CCC' and a destination of 'DDD'
		And reckey '2' leg '1' has an origin of 'YYY' and a destination of 'XXX'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'ZZZ'
		And I have destination criteria of 'BBB'
		And I 'do' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on destination criteria
	Then I have '3' total records
		And reckey '1' exists
		And reckey '2' does not exist
		And a record with origin 'AAA' exists
		And a record with origin 'BBB' exists
		And a record with origin 'CCC' exists

Scenario: GetDataBasedOnDestinationCriteria_ReturnAllLegs_NotIn_LegWithinTripMatchesFilter_DontReturnAnyLegsForThatTrip
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB' 
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '1' leg '3' has an origin of 'CCC' and a destination of 'DDD'
		And reckey '2' leg '1' has an origin of 'YYY' and a destination of 'XXX'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'ZZZ'
		And I have destination criteria of 'BBB'
		And I 'do' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on destination criteria
	Then I have '2' total records
		And reckey '1' does not exist
		And reckey '2' exists
		And a record with origin 'YYY' exists
		And a record with origin 'XXX' exists

Scenario: GetDataBasedOnDestinationCriteria_DontReturnAllLegs_IsIn_LegWithinTripMatchesFilter_ReturnThatLeg
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB' 
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '1' leg '3' has an origin of 'CCC' and a destination of 'DDD'
		And reckey '2' leg '1' has an origin of 'YYY' and a destination of 'XXX'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'ZZZ'
		And I have destination criteria of 'BBB'
		And I 'do not' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on destination criteria
	Then I have '1' total records
		And reckey '1' exists
		And reckey '2' does not exist
		And a record with origin 'AAA' exists

Scenario: GetDataBasedOnDestinationCriteria_DontReturnAllLegs_NotIn_ReturnAllLegsOfThatDoNotMatchFilter
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB' 
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '1' leg '3' has an origin of 'CCC' and a destination of 'DDD'
		And reckey '2' leg '1' has an origin of 'YYY' and a destination of 'XXX'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'ZZZ'
		And I have destination criteria of 'BBB'
		And I 'do not' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on destination criteria
	Then I have '4' total records
		And reckey '1' exists
		And reckey '2' exists
		And a record with origin 'BBB' exists
		And a record with origin 'CCC' exists
		And a record with origin 'YYY' exists
		And a record with origin 'XXX' exists

Scenario: GetDataBasedOnDestinationCriteria__MultipleCriteria_ReturnAllLegs_IsIn_LegWithinATripMatchesFilter_ReturnAllLegsOfThatTrip
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB' 
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '1' leg '3' has an origin of 'CCC' and a destination of 'DDD'
		And reckey '2' leg '1' has an origin of 'YYY' and a destination of 'XXX'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'ZZZ'
		And reckey '3' leg '1' has an origin of 'EEE' and a destination of 'FFF'
		And I have destination criteria of 'BBB'
		And I have destination criteria of 'FFF'
		And I 'do' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on destination criteria
	Then I have '4' total records
		And reckey '1' exists
		And reckey '2' does not exist
		And reckey '3' exists
		And a record with origin 'AAA' exists
		And a record with origin 'BBB' exists
		And a record with origin 'CCC' exists
		And a record with origin 'EEE' exists

#Scenario: GetDataBasedOnDestinationAndOriginCriteria_ReturnAllLegs_IsIn_ReturnAllLegsOfTripsThatMatchOriginAndDestinationInAtLeastOneLeg
#	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
#		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
#		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
#		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
#		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
#		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
#		And I have origin criteria of 'AAA'
#		And I have destination criteria of 'BBB'
#		And I 'do' want to return all legs
#		And I want data that 'is' in the criteria
#	When I filter the data on origin and destination criteria
#	Then I have '2' total records
#		And reckey '1' exists
#		And reckey '2' does not exist
#		And reckey '3' does not exist
#		And reckey '4' does not exist
#		And a record with origin 'AAA' exists
#		And a record with origin 'BBB' exists

Scenario: GetDataBasedOnDestinationAndOriginCriteria_ReturnAllLegs_NotIn_DontReturnAnyTripsThatMatchOriginAndDestinationInAtLeastOneLeg
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And I have origin criteria of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin and destination criteria
	Then I have '4' total records
		And reckey '1' does not exist
		And reckey '2' exists
		And reckey '3' exists
		And reckey '4' exists
		And a record with origin 'AAA' exists
		And a record with origin 'XXX' exists
		And a record with origin 'ZZZ' exists
		And a record with origin 'TTT' exists

Scenario: GetDataBasedOnDestinationAndOriginCriteria_DontReturnAllLegs_IsIn_ReturnAnyLegOfAnyTripThatMatchesOriginAndDestination
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And I have origin criteria of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do not' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on origin and destination criteria
	Then I have '1' total records
		And reckey '1' exists
		And reckey '2' does not exist
		And reckey '3' does not exist
		And reckey '4' does not exist
		And a record with origin 'AAA' exists

Scenario: GetDataBasedOnDestinationAndOriginCriteria_DontReturnAllLegs_NotIn_ReturnAllLegsThatDoNotMatchBothOriginAndDestinationFilter
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And I have origin criteria of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do not' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin and destination criteria
	Then I have '5' total records
		And reckey '1' exists
		And reckey '2' exists
		And reckey '3' exists
		And reckey '4' exists
		And a record with origin 'BBB' exists
		And a record with origin 'AAA' exists
		And a record with origin 'XXX' exists
		And a record with origin 'ZZZ' exists
		And a record with origin 'TTT' exists

Scenario: GetDataBasedOnDestinationOrOriginCriteria_ReturnAllLegs_IsIn_ReturnAllLegsOfTripsWhereOriginOrDestinationMatchesCriteriaInAtLeastOneLeg
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'DDD' and a destination of 'AAA'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And I have criteria of 'AAA'
		And I 'do' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on origin or destination criteria
	Then I have '4' total records
		And reckey '1' exists
		And reckey '2' exists
		And reckey '3' does not exist
		And reckey '4' does not exist
		And a record with origin 'AAA' exists
		And a record with origin 'BBB' exists
		And a record with origin 'DDD' exists
		And a record with origin 'XXX' exists

Scenario: GetDataBasedOnDestinationOrOriginCriteria_ReturnAllLegs_NotIn_DontReturnAnyTripsWhereCriteriaMatchesOriginOrDestinationInAtLeastOneLeg
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'DDD' and a destination of 'AAA'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '3' leg '2' has an origin of 'TTT' and a destination of 'SSS'
		And I have criteria of 'AAA'
		And I 'do' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin or destination criteria
	Then I have '2' total records
		And reckey '1' does not exist
		And reckey '2' does not exist
		And reckey '3' exists
		And a record with origin 'ZZZ' exists
		And a record with origin 'TTT' exists

Scenario: GetDataBasedOnDestinationOrOriginCriteria_DontReturnAllLegs_IsIn_ReturnAnyLegOfAnyTripWhereCriteriaMatchesOriginOrDestination
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'DDD' and a destination of 'AAA'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And I have criteria of 'AAA'
		And I 'do not' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on origin or destination criteria
	Then I have '2' total records
		And reckey '1' exists
		And reckey '2' exists
		And reckey '3' does not exist
		And reckey '4' does not exist
		And a record with origin 'AAA' exists
		And a record with origin 'DDD' exists

#Scenario: GetDataBasedOnDestinationOrOriginCriteria_DontReturnAllLegs_NotIn_ReturnAnyLegOfAnyTripWhereCriteriaDoesNotMatchOriginOrDestination
#	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
#		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
#		And reckey '2' leg '1' has an origin of 'DDD' and a destination of 'AAA'
#		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
#		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
#		And reckey '4' leg '1' has an origin of 'AAA' and a destination of 'AAA'
#		And I have criteria of 'AAA'
#		And I 'do not' want to return all legs
#		And I want data that 'is not' in the criteria
#	When I filter the data on origin or destination criteria
#	Then I have '5' total records
#		And reckey '1' exists
#		And reckey '2' exists
#		And reckey '3' exists
#		And reckey '4' does not exist
#		And a record with origin 'AAA' exists
#		And a record with origin 'BBB' exists
#		And a record with origin 'DDD' exists
#		And a record with origin 'XXX' exists
#		And a record with origin 'ZZZ' exists

