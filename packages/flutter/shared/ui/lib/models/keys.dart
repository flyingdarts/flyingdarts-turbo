import 'package:flutter/material.dart';

enum Keys {
  // AppBar
  appBar,
  appBarBtnSettings,
  appBarBtnLanguage,

  // SpeechRecognition
  speechRecognitionBtn,

  // ErrorDialog
  errorDialogLanguage,
  errorDialogLanguageTitle,
  errorDialogLanguageError,

  // Authentication
  loginWithFacebookBtn,
}

extension KeysExtension on Keys {
  String get value {
    switch (this) {
      // AppBar
      case Keys.appBar:
        return 'APP-BAR';
      case Keys.appBarBtnSettings:
        return 'APP-BAR-BTN-SETTINGS';
      case Keys.appBarBtnLanguage:
        return 'APP-BAR-BTN-LANGUAGE';

      // SpeechRecognition
      case Keys.speechRecognitionBtn:
        return 'SPEECH-RECOGNITION-BUTTON';

      // ErrorDialog
      case Keys.errorDialogLanguage:
        return 'ERROR-DIALOG-LANGUAGE';
      case Keys.errorDialogLanguageTitle:
        return 'ERROR-DIALOG-LANGUAGE-TITLE';
      case Keys.errorDialogLanguageError:
        return 'ERROR-DIALOG-LANGUAGE-ERROR';

      // Authentication
      case Keys.loginWithFacebookBtn:
        return 'LOGIN-WITH-FACEBOOK-BTN';
    }
  }

  Key get key => Key(value);
}
