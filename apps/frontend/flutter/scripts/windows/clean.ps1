Remove-Item -Path pubspec.lock -Force
flutter clean
dart run build_runner clean
flutter pub get
dart run build_runner build