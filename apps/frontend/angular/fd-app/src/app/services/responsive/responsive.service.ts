import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface ResponsiveState {
  isMobile: boolean;
  isTablet: boolean;
  isDesktop: boolean;
  isLandscape: boolean;
  isPortrait: boolean;
  screenWidth: number;
  screenHeight: number;
}

@Injectable({
  providedIn: 'root',
})
export class ResponsiveService {
  private readonly responsiveStateSubject = new BehaviorSubject<ResponsiveState>({
    isMobile: false,
    isTablet: false,
    isDesktop: false,
    isLandscape: false,
    isPortrait: false,
    screenWidth: 0,
    screenHeight: 0,
  });

  public readonly responsiveState$: Observable<ResponsiveState> = this.responsiveStateSubject.asObservable();

  // Breakpoints
  private readonly MOBILE_BREAKPOINT = 768;
  private readonly TABLET_BREAKPOINT = 1024;
  private readonly DESKTOP_BREAKPOINT = 1200;

  constructor() {
    this.initializeResponsiveState();
    this.setupResizeListener();
  }

  private initializeResponsiveState(): void {
    this.updateResponsiveState();
  }

  private setupResizeListener(): void {
    window.addEventListener('resize', () => {
      this.updateResponsiveState();
    });

    window.addEventListener('orientationchange', () => {
      // Add a small delay to ensure orientation change is complete
      setTimeout(() => {
        this.updateResponsiveState();
      }, 100);
    });
  }

  private updateResponsiveState(): void {
    const screenWidth = window.innerWidth;
    const screenHeight = window.innerHeight;
    const isLandscape = screenWidth > screenHeight;
    const isPortrait = screenHeight > screenWidth;

    const isMobile = screenWidth < this.MOBILE_BREAKPOINT;
    const isTablet = screenWidth >= this.MOBILE_BREAKPOINT && screenWidth < this.TABLET_BREAKPOINT;
    const isDesktop = screenWidth >= this.TABLET_BREAKPOINT; // Changed from DESKTOP_BREAKPOINT to TABLET_BREAKPOINT

    this.responsiveStateSubject.next({
      isMobile,
      isTablet,
      isDesktop,
      isLandscape,
      isPortrait,
      screenWidth,
      screenHeight,
    });
  }

  // Helper methods for specific device checks
  public isMobileDevice(): boolean {
    return this.responsiveStateSubject.value.isMobile;
  }

  public isTabletDevice(): boolean {
    return this.responsiveStateSubject.value.isTablet;
  }

  public isDesktopDevice(): boolean {
    return this.responsiveStateSubject.value.isDesktop;
  }

  public isLandscapeOrientation(): boolean {
    return this.responsiveStateSubject.value.isLandscape;
  }

  public isPortraitOrientation(): boolean {
    return this.responsiveStateSubject.value.isPortrait;
  }

  // Method to check if we should show desktop layout (desktop or tablet landscape)
  public shouldShowDesktopLayout(): boolean {
    const state = this.responsiveStateSubject.value;
    return state.isDesktop || (state.isTablet && state.isLandscape);
  }

  // Method to check if we should show mobile layout (mobile or tablet portrait)
  public shouldShowMobileLayout(): boolean {
    const state = this.responsiveStateSubject.value;
    return state.isMobile || (state.isTablet && state.isPortrait);
  }

  // Method to ensure we always have a fallback layout
  public shouldShowAnyLayout(): boolean {
    return this.shouldShowDesktopLayout() || this.shouldShowMobileLayout();
  }
}
