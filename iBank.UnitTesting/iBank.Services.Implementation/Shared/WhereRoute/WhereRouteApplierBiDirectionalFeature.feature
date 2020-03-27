Feature: WhereRouteApplierBiDirectionalFeature
	In order to retrieve subsets of data correctly
	As a user
	I need to be able to filter a dataset based on leg and trip criteria
	
Scenario: GetDataBasedOnDestinationAndOriginCriteria_ReturnAllLegs_IsIn_ReturnAllLegsOfTripsThatAtLeastOneLegMatchOriginAndDestinationInBiDirection
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And reckey '5' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have origin criteria of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on origin and destination criteria
	Then I have '3' total records
		And reckey '1' and leg '1' exists
		And reckey '1' and leg '2' exists
		And reckey '2' and leg '1' does not exist
		And reckey '2' and leg '2' does not exist
		And reckey '3' and leg '1' does not exist
		And reckey '4' and leg '1' does not exist
		And reckey '5' and leg '1' exists

Scenario: GetDataBasedOnDestinationAndOriginCriteria_DontReturnAllLegs_IsIn_ReturnAnyLegOfAnyTripThatMatchOriginAndDestinationInBiDirection
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And reckey '5' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have origin criteria of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do not' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on origin and destination criteria
	Then I have '2' total records
		And reckey '1' and leg '1' exists
		And reckey '1' and leg '2' does not exist
		And reckey '2' and leg '1' does not exist
		And reckey '2' and leg '2' does not exist
		And reckey '3' and leg '1' does not exist
		And reckey '4' and leg '1' does not exist
		And reckey '5' and leg '1' exists

Scenario: GetDataBasedOnDestinationAndOriginCriteria_ReturnAllLegs_NotIn_ReturnTripsThatNotASingleLegMatchOriginAndDestinationInBiDirection
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And reckey '5' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have origin criteria of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin and destination criteria
	Then I have '4' total records
		And reckey '1' and leg '1' does not exist
		And reckey '1' and leg '2' does not exist
		And reckey '2' and leg '1' exists
		And reckey '2' and leg '2' exists
		And reckey '3' and leg '1' exists
		And reckey '4' and leg '1' exists
		And reckey '5' and leg '1' does not exist
		
Scenario: GetDataBasedOnDestinationAndOriginCriteria_DontReturnAllLegs_NotIn_ReturnAllLegsThatDoNotMatchBothOriginAndDestinationInBiDirection
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And reckey '5' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have origin criteria of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do not' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin and destination criteria
	Then I have '5' total records
		And reckey '1' and leg '1' does not exist
		And reckey '1' and leg '2' exists
		And reckey '2' and leg '1' exists
		And reckey '2' and leg '2' exists
		And reckey '3' and leg '1' exists
		And reckey '4' and leg '1' exists
		And reckey '5' and leg '1' does not exist

Scenario: GetDataBasedOnOriginOrDestinationCriteria_ReturnAllLegs_IsIn_ReturnLegsOfTripsThatAtLeastOneLegMatchEitherOriginOrDestination
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And reckey '5' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have origin criteria of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on origin or destination criteria
	Then I have '6' total records
		And reckey '1' and leg '1' exists
		And reckey '1' and leg '2' exists
		And reckey '2' and leg '1' exists
		And reckey '2' and leg '2' exists
		And reckey '3' and leg '1' exists
		And reckey '4' and leg '1' does not exist
		And reckey '5' and leg '1' exists


Scenario: GetDataBasedOnOriginOrDestinationCriteria_ReturnAllLegs_NotIn_ReturnLegsOfTripsThatNotEvenOneLegMatchEitherOriginNorDestination
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And reckey '5' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have origin criteria of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin or destination criteria
	Then I have '1' total records
		And reckey '1' and leg '1' does not exist
		And reckey '1' and leg '2' does not exist
		And reckey '2' and leg '1' does not exist
		And reckey '2' and leg '2' does not exist
		And reckey '3' and leg '1' does not exist
		And reckey '4' and leg '1' exists
		And reckey '5' and leg '1' does not exist

Scenario: GetDataBasedOnOriginOrDestinationCriteria_DontReturnAllLegs_IsIn_ReturnLegsThatEitherMatchOriginOrDestination
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And reckey '5' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have origin criteria of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do not' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on origin or destination criteria
	Then I have '5' total records
		And reckey '1' and leg '1' exists
		And reckey '1' and leg '2' exists
		And reckey '2' and leg '1' exists
		And reckey '2' and leg '2' does not exist
		And reckey '3' and leg '1' exists
		And reckey '4' and leg '1' does not exist
		And reckey '5' and leg '1' exists


Scenario: GetDataBasedOnOriginOrDestinationCriteria_DontReturnAllLegs_NotIn_ReturnLegsOfTripsThatNotEvenOneLegMatchEitherOriginNorDestination
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And reckey '5' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have origin criteria of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do not' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin or destination criteria
	Then I have '2' total records
		And reckey '1' and leg '1' does not exist
		And reckey '1' and leg '2' does not exist
		And reckey '2' and leg '1' does not exist
		And reckey '2' and leg '2' exists
		And reckey '3' and leg '1' does not exist
		And reckey '4' and leg '1' exists
		And reckey '5' and leg '1' does not exist

#This is would satisfy both origin and destination because of bi directional		
Scenario: GetDataBasedOnOriginOrDestinationCriteria_DontReturnAllLegs_NotIn_NoOrigin_ReturnLegsOfTripThatMatchesOriginOrDestination
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And reckey '5' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have destination criteria of 'BBB'
		And I 'do not' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin or destination criteria
	Then I have '3' total records
		And reckey '1' and leg '1' does not exist
		And reckey '1' and leg '2' does not exist
		And reckey '2' and leg '1' exists
		And reckey '2' and leg '2' exists
		And reckey '3' and leg '1' does not exist
		And reckey '4' and leg '1' exists
		And reckey '5' and leg '1' does not exist


#This is would satisfy both origin and destination because of bi directional		
Scenario: GetDataBasedOnOriginOrDestinationCriteria_DontReturnAllLegs_IsIn_NoOrigin_ReturnLegsOfTripThatMatchesOriginOrDestination
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have destination criteria of 'AAA'
		And I 'do not' want to return all legs
		And I want data that 'is' in the criteria
	When I filter the data on origin or destination criteria
	Then I have '3' total records
		And reckey '1' and leg '1' exists
		And reckey '1' and leg '2' does not exist
		And reckey '2' and leg '1' exists
		And reckey '2' and leg '2' does not exist
		And reckey '3' and leg '1' does not exist
		And reckey '4' and leg '1' exists


#This is would satisfy both origin and destination because of bi directional		
Scenario: GetDataBasedOnOriginOrDestinationCriteria_ReturnAllLegs_NotIn_NoOrigin_ReturnLegsOfTripThatDontMatchesOriginAndDestination
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'TTT' and a destination of 'SSS'
		And reckey '5' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have origin criteria of 'AAA'
		And I 'do' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin or destination criteria
	Then I have '2' total records
		And reckey '1' and leg '1' does not exist
		And reckey '1' and leg '2' does not exist
		And reckey '2' and leg '1' does not exist
		And reckey '2' and leg '2' does not exist
		And reckey '3' and leg '1' exists
		And reckey '4' and leg '1' exists
		And reckey '5' and leg '1' does not exist

#This is would satisfy both origin and destination because of bi directional		
Scenario: GetDataBasedOnOriginOrDestinationCriteria_ReturnAllLegs_NotIn_NoOrigin_ReturnTripThatDoesntMatchesEitherOriginOrDestination
	Given reckey '1' leg '1' has an origin of 'AAA' and a destination of 'BBB'
		And reckey '1' leg '2' has an origin of 'BBB' and a destination of 'CCC'
		And reckey '2' leg '1' has an origin of 'AAA' and a destination of 'ZZZ'
		And reckey '2' leg '2' has an origin of 'XXX' and a destination of 'YYY'
		And reckey '3' leg '1' has an origin of 'ZZZ' and a destination of 'BBB'
		And reckey '4' leg '1' has an origin of 'BBB' and a destination of 'AAA'
		And I have destination criteria of 'AAA'
		And I 'do' want to return all legs
		And I want data that 'is not' in the criteria
	When I filter the data on origin or destination criteria
	Then I have '1' total records
		And reckey '1' and leg '1' does not exist
		And reckey '1' and leg '2' does not exist
		And reckey '2' and leg '1' does not exist
		And reckey '2' and leg '2' does not exist
		And reckey '3' and leg '1' exists
		And reckey '4' and leg '1' does not exist