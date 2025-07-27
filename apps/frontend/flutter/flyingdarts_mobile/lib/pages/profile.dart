import 'package:flutter/material.dart' hide BackButton;
import 'package:ui/ui.dart';

class ProfilePage extends StatelessWidget {
  const ProfilePage({super.key});

  @override
  Widget build(BuildContext context) {
    return const FlyingdartsScaffold(
      leading: BackButton(),
      child: Text('Profile'),
    );
  }
}
