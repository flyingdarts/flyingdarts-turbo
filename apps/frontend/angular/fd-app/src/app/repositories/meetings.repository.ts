import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';

@Injectable({ providedIn: 'root' })
export class MeetingsRepository {
  private baseUrl = `${environment.dyteApi}/meetings`;

  constructor(private readonly http: HttpClient) {}

  createMeeting(title: string): Observable<MeetingResponse> {
    const body = { title: title };

    return this.http.post<MeetingResponse>(this.baseUrl, body);
  }

  addParticipant(
    meetingId: string,
    name: string,
    presetName: string
  ): Observable<ParticipantResponse> {
    const url = `${this.baseUrl}/${meetingId}/participants`;
    const body = {
      name: name,
      preset_name: presetName,
    };
    return this.http.post<ParticipantResponse>(url, body);
  }
}

// Interfaces for the responses
export interface MeetingResponse {
  success: boolean;
  data: {
    id: string;
    title: string;
    record_on_start: boolean;
    live_stream_on_start: boolean;
    persist_chat: boolean;
    summarize_on_end: boolean;
    is_large: boolean;
    status: string;
    created_at: string;
    updated_at: string;
  };
}

export interface ParticipantResponse {
  success: boolean;
  data: {
    token: string;
    id: string;
    name: string;
    custom_participant_id: string;
    preset_id: string;
    sip_enabled: boolean;
    created_at: string;
    updated_at: string;
  };
}
