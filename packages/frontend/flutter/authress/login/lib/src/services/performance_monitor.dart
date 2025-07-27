import 'dart:async';
import 'package:flutter/foundation.dart';

/// Performance monitoring service for tracking authentication operations
class PerformanceMonitor {
  static PerformanceMonitor? _instance;
  static PerformanceMonitor get instance => _instance ??= PerformanceMonitor._();
  
  PerformanceMonitor._();

  final Map<String, Stopwatch> _activeTimers = {};
  final Map<String, List<Duration>> _completedOperations = {};
  final Map<String, int> _operationCounts = {};
  bool _enabled = kDebugMode;

  /// Enable or disable performance monitoring
  void setEnabled(bool enabled) {
    _enabled = enabled;
  }

  /// Start timing an operation
  void startTimer(String operationName) {
    if (!_enabled) return;

    final stopwatch = Stopwatch()..start();
    _activeTimers[operationName] = stopwatch;
    
    debugPrint('⏱️ PerformanceMonitor: Started timing $operationName');
  }

  /// Stop timing an operation and record the result
  Duration? stopTimer(String operationName) {
    if (!_enabled) return null;

    final stopwatch = _activeTimers.remove(operationName);
    if (stopwatch == null) {
      debugPrint('⚠️ PerformanceMonitor: No active timer for $operationName');
      return null;
    }

    stopwatch.stop();
    final duration = stopwatch.elapsed;

    // Record the operation
    _completedOperations.putIfAbsent(operationName, () => []).add(duration);
    _operationCounts[operationName] = (_operationCounts[operationName] ?? 0) + 1;

    debugPrint('✅ PerformanceMonitor: $operationName completed in ${duration.inMilliseconds}ms');

    // Log warning for slow operations
    if (duration.inMilliseconds > 5000) {
      debugPrint('🐌 PerformanceMonitor: WARNING - $operationName took ${duration.inSeconds}s (slow operation)');
    }

    return duration;
  }

  /// Record a custom metric
  void recordMetric(String metricName, double value, String unit) {
    if (!_enabled) return;
    debugPrint('📊 PerformanceMonitor: $metricName: $value $unit');
  }

  /// Get performance statistics for an operation
  PerformanceStats? getStats(String operationName) {
    if (!_enabled) return null;

    final durations = _completedOperations[operationName];
    if (durations == null || durations.isEmpty) return null;

    final count = durations.length;
    final totalMs = durations.fold(0, (sum, duration) => sum + duration.inMilliseconds);
    final avgMs = totalMs / count;

    durations.sort((a, b) => a.inMilliseconds.compareTo(b.inMilliseconds));
    final p50Ms = durations[count ~/ 2].inMilliseconds;
    final p95Ms = durations[(count * 0.95).round() - 1].inMilliseconds;
    final minMs = durations.first.inMilliseconds;
    final maxMs = durations.last.inMilliseconds;

    return PerformanceStats(
      operationName: operationName,
      count: count,
      averageMs: avgMs.round(),
      p50Ms: p50Ms,
      p95Ms: p95Ms,
      minMs: minMs,
      maxMs: maxMs,
    );
  }

  /// Get all performance statistics
  Map<String, PerformanceStats> getAllStats() {
    if (!_enabled) return {};

    final stats = <String, PerformanceStats>{};
    for (final operation in _completedOperations.keys) {
      final stat = getStats(operation);
      if (stat != null) {
        stats[operation] = stat;
      }
    }
    return stats;
  }

  /// Print a performance report
  void printReport() {
    if (!_enabled) return;

    debugPrint('\n📊 === AUTHRESS PERFORMANCE REPORT ===');
    final stats = getAllStats();
    
    if (stats.isEmpty) {
      debugPrint('No performance data recorded');
      return;
    }

    stats.forEach((operation, stat) {
      debugPrint('🔍 $operation:');
      debugPrint('   Count: ${stat.count}');
      debugPrint('   Average: ${stat.averageMs}ms');
      debugPrint('   Median (P50): ${stat.p50Ms}ms');
      debugPrint('   P95: ${stat.p95Ms}ms');
      debugPrint('   Range: ${stat.minMs}ms - ${stat.maxMs}ms');
      debugPrint('');
    });

    // Identify potential issues
    final slowOperations = stats.entries
        .where((entry) => entry.value.averageMs > 3000)
        .map((entry) => entry.key)
        .toList();

    if (slowOperations.isNotEmpty) {
      debugPrint('⚠️ SLOW OPERATIONS (>3s average):');
      for (final operation in slowOperations) {
        debugPrint('   • $operation: ${stats[operation]!.averageMs}ms avg');
      }
    }

    debugPrint('=======================================\n');
  }

  /// Clear all performance data
  void clearData() {
    _completedOperations.clear();
    _operationCounts.clear();
    _activeTimers.clear();
    debugPrint('🗑️ PerformanceMonitor: Data cleared');
  }

  /// Track a future operation automatically
  Future<T> trackOperation<T>(String operationName, Future<T> Function() operation) async {
    startTimer(operationName);
    try {
      final result = await operation();
      stopTimer(operationName);
      return result;
    } catch (e) {
      stopTimer(operationName);
      debugPrint('❌ PerformanceMonitor: $operationName failed: $e');
      rethrow;
    }
  }

  /// Track a synchronous operation
  T trackSync<T>(String operationName, T Function() operation) {
    startTimer(operationName);
    try {
      final result = operation();
      stopTimer(operationName);
      return result;
    } catch (e) {
      stopTimer(operationName);
      debugPrint('❌ PerformanceMonitor: $operationName failed: $e');
      rethrow;
    }
  }
}

/// Performance statistics for an operation
class PerformanceStats {
  final String operationName;
  final int count;
  final int averageMs;
  final int p50Ms;
  final int p95Ms;
  final int minMs;
  final int maxMs;

  const PerformanceStats({
    required this.operationName,
    required this.count,
    required this.averageMs,
    required this.p50Ms,
    required this.p95Ms,
    required this.minMs,
    required this.maxMs,
  });

  @override
  String toString() {
    return 'PerformanceStats(operation: $operationName, count: $count, avg: ${averageMs}ms, p50: ${p50Ms}ms, p95: ${p95Ms}ms)';
  }
}

/// Mixin for classes that want to easily track performance
mixin PerformanceTrackingMixin {
  PerformanceMonitor get _monitor => PerformanceMonitor.instance;

  /// Track an async operation
  Future<T> trackPerformance<T>(String operationName, Future<T> Function() operation) {
    return _monitor.trackOperation(operationName, operation);
  }

  /// Track a sync operation
  T trackSyncPerformance<T>(String operationName, T Function() operation) {
    return _monitor.trackSync(operationName, operation);
  }

  /// Record a custom metric
  void recordMetric(String metricName, double value, [String unit = 'units']) {
    _monitor.recordMetric(metricName, value, unit);
  }
} 