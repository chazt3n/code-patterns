Feature: Age as a value
	In order to avoid constantly making the same calculations over and over
	As a constant comparer of the ages of things
	I want to be told the age of something based on a value that's easier to obtain and store

Scenario Outline: Conversion from DateTime
	Given I am convinced that the current date is <some specific point in time>
	And I have a birth date to examine that is <some specific date of birth>
	When I convert the birth date to an age value
	Then the resulting age should have <years> years, <months> months, and <days> days
Examples:
	| some specific point in time | some specific date of birth | years | months | days |
	| 8/1/2013                    | 8/1/1973                    | 40    | 0      | 0    |
	| 2/14/2010                   | 2/1/2000                    | 10    | 0      | 13   |
	| 1/15/2011                   | 12/15/2010                  | 0     | 1      | 0    |

# TODO: for issue #56, expand on this to capture previously observed anomaly points;
# the main breakpoint will likely need to be in ValueExtensions.cs, line 46.
# Look for occasions where the day value is less than zero. Month gets decremented
# in this case but day remains negative; that's the scenario we're currently trying
# to reproduce.

Scenario: Conversion from TimeSpan

# TODO: spec this out