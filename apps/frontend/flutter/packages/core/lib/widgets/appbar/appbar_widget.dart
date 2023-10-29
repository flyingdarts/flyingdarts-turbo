import 'package:amplify_flutter/amplify_flutter.dart';
import 'package:flutter/material.dart';
import 'package:flyingdarts_features/language/dialog/language_dialog.dart';
import 'package:flyingdarts_features/language/state/language_cubit.dart';
import 'package:provider/provider.dart';

import '../../navigation/navigation_cubit.dart';
import 'button/appbar_button_widget.dart';

class MyAppBar extends StatelessWidget implements PreferredSizeWidget {
  const MyAppBar({
    Key? key,
  }) : super(key: key);

  @override
  Widget build(BuildContext context) {
    var cubit = Provider.of<NavigationCubit>(context, listen: false);
    return AppBar(
      elevation: 0,
      title: const Text(
        'Flyingdarts',
        style: TextStyle(
          fontFamily: 'Poppins',
          fontSize: 20,
          fontWeight: FontWeight.w400,
          color: Colors.white,
        ),
      ),
      actions: cubit.state.isLoggedIn ? _buildAppBarButtons(context) : null,
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(kToolbarHeight);

  List<Widget> _buildAppBarButtons(BuildContext context) {
    return <Widget>[
      AppBarButton(
        key: const Key("SettingsButton"),
        iconData: Icons.settings,
        onPressed: () => {
          showDialog<void>(
            context: context,
            builder: (dContext) => LanguageDialog(
              languages: context.read<LanguageCubit>().state.availableLanguages,
            ),
          )
        },
      ),
      AppBarButton(
        key: const Key("LogoutButton"),
        iconData: Icons.logout,
        onPressed: () async {
          var cubit = Provider.of<NavigationCubit>(context, listen: false);
          cubit.setIsLoading(true);
          try {
            await Amplify.Auth.signOut();
            // Sign out successful
            // Redirect or navigate to the desired page
          } on AuthException catch (e) {
            // Sign out failed
            // Handle the exception or display an error message
            print('Sign out failed: ${e.message}');
          }
        },
      ),
    ];
  }
}
