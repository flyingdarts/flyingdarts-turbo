@requires-logged-in-user
Feature: Onboarding

    Scenario: I can register my profile
        Given the onboarding profile page is loaded
        Then I can fill in Socrates as my Nickname
        And I can fill in mike@flyingdarts.net as my Email
        And I can select Belgium as my Country