import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:internationalization/internationalization.dart';
import 'package:ui/ui.dart';

import '../state/profile_cubit.dart';

class ProfileDialog extends StatelessWidget {
  const ProfileDialog({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    var cubit = GetIt.I<ProfileCubit>();
    return SimpleDialog(
      key: const Key("ProfileDialog"),
      backgroundColor: MyTheme.primaryColor,
      contentPadding: const EdgeInsets.fromLTRB(0.0, 8.0, 0.0, 20.0),
      titlePadding: const EdgeInsets.only(
        bottom: 24,
        top: 24,
        left: 24,
        right: 24,
      ),
      title: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          const Text(
            'Profile', // TODO: AppLocalizations.of(context).profile
            textAlign: TextAlign.center,
          ),
          Align(
            key: const Key("ProfileCloseDialog"),
            alignment: Alignment.topRight,
            child: InkWell(
              borderRadius: BorderRadius.circular(4),
              onTap: () => Navigator.pop(context),
              child: const Padding(
                padding: EdgeInsets.all(8.0),
                child: Icon(
                  Icons.close,
                  size: 16,
                ),
              ),
            ),
          ),
        ],
      ),
      children: [
        Column(
          children: [
            Row(
              mainAxisSize: MainAxisSize.max,
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Padding(
                  padding: EdgeInsets.only(right: 5),
                  child: Text("Username:"),
                ),
                Text(cubit.state.userName)
              ],
            ),
            Row(
              mainAxisSize: MainAxisSize.max,
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Padding(
                  padding: EdgeInsets.only(right: 5),
                  child: Text("Email:"),
                ),
                Text(cubit.state.email)
              ],
            ),
            Row(
              mainAxisSize: MainAxisSize.max,
              mainAxisAlignment: MainAxisAlignment.center,
              children: [
                const Padding(
                  padding: EdgeInsets.only(right: 5),
                  child: Text("Country:"),
                ),
                Text(cubit.state.country)
              ],
            ),
          ],
        ),
        SizedBox(
          height: 40,
        ),
        Opacity(
          opacity: 0.6,
          child: Row(
            mainAxisSize: MainAxisSize.max,
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Text(cubit.state.userId),
            ],
          ),
        ),
      ],
    );
  }
}
