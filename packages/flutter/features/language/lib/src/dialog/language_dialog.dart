import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:speech_to_text/speech_to_text.dart';
import 'package:ui/ui.dart';

import '../state/language_cubit.dart';

class LanguageDialog extends StatelessWidget {
  const LanguageDialog({Key? key}) : super(key: key);

  @override
  Widget build(BuildContext context) {
    var cubit = GetIt.I<LanguageCubit>();
    var languages = cubit.state.availableLanguages;

    return SimpleDialog(
      key: const Key("LanguageDialog"),
      backgroundColor: MyTheme.primaryColor,
      contentPadding: const EdgeInsets.fromLTRB(0.0, 8.0, 0.0, 32.0),
      titlePadding: const EdgeInsets.all(24.0),
      title: Row(
        mainAxisAlignment: MainAxisAlignment.spaceBetween,
        children: [
          const Text(
            'Languages',
            textAlign: TextAlign.center,
          ),
          Align(
            key: const Key("LanguageCloseDialog"),
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
        Container(
          height: MediaQuery.of(context).size.height / 2,
          child: SingleChildScrollView(
            child: Row(
              mainAxisAlignment: MainAxisAlignment.spaceEvenly,
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Expanded(
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      ...languages
                          .map(
                            (LocaleName item) => Column(
                              children: [
                                Padding(
                                  padding: EdgeInsets.only(left: 20, right: 20, top: 0),
                                  child: MaterialButton(
                                      onPressed: () {
                                        cubit.setPreferedLocale(item);
                                        Navigator.pop(context);
                                      },
                                      child: Text(item.name)),
                                ),
                              ],
                            ),
                          )
                          .toList()
                    ],
                  ),
                )
              ],
            ),
          ),
        ),
      ],
    );
  }
}
