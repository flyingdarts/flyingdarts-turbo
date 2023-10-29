import 'package:flutter/material.dart';
import 'package:widgetbook/widgetbook.dart';

import '../models/models.dart';
import 'navigation_tree.dart';
import 'search_field.dart';

class NavigationPanel extends StatefulWidget {
  const NavigationPanel({
    super.key,
    this.initialPath,
    this.onNodeSelected,
    required this.directories,
  });

  final String? initialPath;
  final NodeSelectedCallback? onNodeSelected;
  final List<MultiChildNavigationNodeData> directories;

  @override
  State<NavigationPanel> createState() => _NavigationPanelState();
}

class _NavigationPanelState extends State<NavigationPanel> {
  String searchQuery = '';

  @override
  Widget build(BuildContext context) {
    var state = WidgetbookState.of(context);

    return Container(
      constraints: const BoxConstraints(minWidth: 50, maxWidth: 300),
      child: Card(
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const SizedBox(height: 16),
            Flex(
              direction: Axis.horizontal,
              children: [
                IconButton(
                  iconSize: 30,
                  alignment: Alignment.center,
                  icon: const Icon(
                    Icons.home,
                    size: 28,
                    color: Colors.white,
                  ),
                  onPressed: () => {state.updatePath('')},
                ),
                Expanded(
                  child: SearchField(
                    searchValue: searchQuery,
                    onSearchChanged: (value) {
                      setState(() => searchQuery = value);
                    },
                    onSearchCancelled: () {
                      setState(() => searchQuery = '');
                    },
                  ),
                ),
              ],
            ),
            const SizedBox(height: 16),
            Expanded(
              child: NavigationTree(
                initialPath: widget.initialPath,
                onNodeSelected: widget.onNodeSelected,
                directories: widget.directories,
                searchQuery: searchQuery,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
