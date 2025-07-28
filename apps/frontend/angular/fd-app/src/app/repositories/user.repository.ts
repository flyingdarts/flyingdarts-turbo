import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { UserDto } from '../dtos/user.dto';

@Injectable({ providedIn: 'root' })
export class UserRepository {
  private baseHref = '';

  constructor(private readonly httpClient: HttpClient) {
    this.baseHref = environment.friendsApi;
  }

  public getUser(): Observable<UserDto> {
    return this.httpClient.get<UserDto>(`${this.baseHref}/user`);
  }
}
export interface UserProfileDetails {
  UserId?: string;
  UserName: string;
  Country: string;
  Email: string;

  isRegistered?: boolean;
  cameraPermissionGranted?: boolean;

  AuthProviderUserId?: string;
  Picture?: string;
}
