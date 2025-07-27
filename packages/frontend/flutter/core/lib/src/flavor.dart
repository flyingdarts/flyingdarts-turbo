import 'package:injectable/injectable.dart';

const Environment acc = Environment("acc");

const Named flavor = Named("flavor");

@module
abstract class FlavorModule {
  @dev
  @flavor
  @Singleton(order: -1000)
  String get flavorDev => "dev";

  @prod
  @flavor
  @Singleton(order: -1000)
  String get flavorProd => "prod";

  @acc
  @flavor
  @Singleton(order: -1000)
  String get flavorAcc => "acc";
}
