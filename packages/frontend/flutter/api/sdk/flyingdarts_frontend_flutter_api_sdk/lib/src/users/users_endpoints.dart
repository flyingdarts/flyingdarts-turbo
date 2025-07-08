typedef PathSections = List<String>;

class UsersEndpoints {
  static const PathSections users = ["users"];
  static const PathSections profile = [...users, "profile"];

  static Uri applyPathSections(Uri root, PathSections pathSections) {
    var rootStr = root.toString();
    if (!rootStr.endsWith('/')) {
      rootStr += '/';
    }

    return Uri.parse(rootStr + pathSections.join('/'));
  }
}
