Feature: Time Extensions
	As a developer, I want to have access to extensions that
	allow me to manipulate and access date and time information
	more easily than I can with standard methods.

Scenario: DateTime Accuracy to One Second
	Given I have a DateTime value
	And I have a second DateTime value that varies from the first by 100 milliseconds
	When I adjust the accuracy of each DateTime value to one second
	And I compare the first and second DateTime values
	Then the resulting difference should be zero

Scenario Outline: DateTime as age
	Given I have a DateTime value set <years> years, <days> days, and <months> months in the past
	When I view the DateTime value as age 
	Then the resulting age should be <expected years> years, <expected days> days, and <expected months> months old
	Examples: 
	| years | days | months | expected years | expected days | expected months |
	| 4     | 0    | 0      | 4              | 0               | 0             |
	| 0     | 4    | 0      | 0              | 4               | 0             |
	| 0     | 0    | 4      | 0              | 0               | 4             |
	
	