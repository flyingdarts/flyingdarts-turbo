Feature: Public

    Scenario: Can open the privacy policy page
        Given the login page is loaded
        Then I click on Privacy Policy
        And The Privacy Policy page is opened

    Scenario: Can open the terms of service page
        Given the login page is loaded
        Then I click on Terms of Service
        And The Terms of Service page is opened