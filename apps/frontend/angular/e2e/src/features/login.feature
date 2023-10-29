Feature: Login

    Scenario: I can click the login button
        Given the login page is loaded
        Then I cannot login
        When I select the checkbox
        And I click the login with facebook button
        Then The AWS Oauth window opens