import 'package:flutter/material.dart';
import 'package:flyingdarts_shared/themes/theme.dart';

class LanguageDialog extends StatelessWidget {
  final List<String> languages;
  const LanguageDialog({Key? key, required this.languages}) : super(key: key);

  @override
  Widget build(BuildContext context) {
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
                            (String item) => Column(
                              children: [
                                Padding(
                                  padding: EdgeInsets.only(left: 20, right: 20, top: 5),
                                  child: Text(item),
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
