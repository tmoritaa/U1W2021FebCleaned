using System;
using System.Collections.Generic;
using UnityEngine;

namespace Domain {
  public class Reservation {
    private readonly Dictionary<ReservationType, int> reservationRequests = new Dictionary<ReservationType, int>();
    private ReservationType? curReservationType;

    public Reservation() {
      foreach (ReservationType reservationType in Enum.GetValues(typeof(ReservationType))) {
        this.reservationRequests[reservationType] = 0;
      }
    }

    public void RequestReservation(ReservationType reservationType) {
      this.reservationRequests[reservationType] += 1;
    }

    public bool CanReserve(ReservationType reservationType) {
      var canReserve = false;
      switch (reservationType) {
        case ReservationType.Collector:
          canReserve = !this.curReservationType.HasValue && this.reservationRequests[ReservationType.Scoring] <= 0;
          break;
        case ReservationType.Scoring:
          canReserve = !this.curReservationType.HasValue;
          break;
      }

      return canReserve;
    }

    public bool Reserve(ReservationType reservationType) {
      if (this.reservationRequests[reservationType] <= 0) {
        var warningMsg = $"{reservationType} reserved without reservation request";
        #if UNITY_EDITOR
        throw new NotSupportedException(warningMsg);
        #endif

        Debug.LogWarning(warningMsg);
        return false;
      }

      this.reservationRequests[reservationType] -= 1;

      this.curReservationType = reservationType;

      return true;
    }

    public bool UnReserve(ReservationType reservationType) {
      if (!this.curReservationType.HasValue || this.curReservationType.Value != reservationType) {
        var warningMsg = $"Unreservation of type {reservationType} requested when current reservation is {this.curReservationType}";
        #if UNITY_EDITOR
        throw new NotSupportedException(warningMsg);
        #endif

        Debug.LogWarning(warningMsg);
        return false;
      }

      this.curReservationType = null;

      return true;
    }
  }
}