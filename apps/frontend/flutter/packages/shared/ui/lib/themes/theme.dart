import 'package:flutter/material.dart';
import 'package:flutter/services.dart';

mixin MyTheme {
  static const MaterialColor primaryColor = MaterialColor(0xff3c46a7, <int, Color>{
    50: Color(0xffE1E3F4), //10%
    100: Color(0xffA7ACDF),
    200: Color(0xff8990D5),
    300: Color(0xff6B74CA),
    400: Color(0xff4D57BF),
    500: Color(0xff3C46A7),
    600: Color(0xff2D357E),
    700: Color(0xff1E2354),
    800: Color(0xff10122B),
    900: Color(0xff010102)
  });
  static const MaterialColor secondaryColor = MaterialColor(0xffF9617D, <int, Color>{
    50: Color(0xffFFFFFF),
    100: Color(0xffFFFEFE),
    200: Color(0xffFDD7DE),
    300: Color(0xffFCB0BE),
    400: Color(0xffFA889D),
    500: Color(0xffF9617D),
    600: Color(0xffF72B51),
    700: Color(0xffE10930),
    800: Color(0xffAB0725),
    900: Color(0xff750419),
  });
}

var myTheme = ThemeData(
  useMaterial3: false,
  primaryColor: MyTheme.secondaryColor,
  scaffoldBackgroundColor: MyTheme.primaryColor,
  primaryColorDark: const Color(0xff3C46A7),
  brightness: Brightness.dark,
  fontFamily: 'Poppins',
  appBarTheme: const AppBarTheme(
    systemOverlayStyle: SystemUiOverlayStyle.light,
    elevation: 0,
    color: Colors.transparent,
  ),
  textTheme: const TextTheme(
    bodyText1: TextStyle(
      fontSize: 18,
      fontWeight: FontWeight.w300,
    ),
  ),
  elevatedButtonTheme: ElevatedButtonThemeData(
    style: ElevatedButton.styleFrom(
      primary: MyTheme.secondaryColor,
      minimumSize: const Size(64, 48),
      textStyle: const TextStyle(
        fontFamily: 'Poppins',
        fontSize: 18,
        fontWeight: FontWeight.bold,
      ),
    ),
  ),
  outlinedButtonTheme: OutlinedButtonThemeData(
    style: OutlinedButton.styleFrom(
      primary: MyTheme.primaryColor,
      minimumSize: const Size(64, 48),
      side: const BorderSide(width: 1, color: MyTheme.primaryColor),
      textStyle: const TextStyle(
        fontFamily: 'Poppins',
        fontWeight: FontWeight.bold,
      ),
    ),
  ),
  textButtonTheme: TextButtonThemeData(
    style: TextButton.styleFrom(
      primary: MyTheme.primaryColor,
      minimumSize: const Size(64, 48),
      textStyle: const TextStyle(
        fontFamily: 'Poppins',
        fontWeight: FontWeight.bold,
      ),
    ),
  ),
);
