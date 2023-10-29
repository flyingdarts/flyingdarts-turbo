@requires-registered-user
Feature: Lobby
    
    Scenario: I can visit the lobby when im logged in
        Given the lobby page is loaded
        Then I can see Socrates in the nav-bar

    Scenario: When i click my name the account/profile page opens
        Given the lobby page is loaded
        When I click on the profile button
        Then the profile page is loaded

    Scenario: When i click my name the account/settings page opens
        Given the lobby page is loaded
        When I click on the settings button
        Then the settings page is loaded