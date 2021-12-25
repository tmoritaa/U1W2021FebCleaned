namespace Domain {
  public struct Piece {
    public readonly int Id;

    public Piece(int id) {
      this.Id = id;
    }

    public override int GetHashCode() {
      return this.Id.GetHashCode();
    }

    public override bool Equals(object obj) {
      if (obj is Piece p) {
        return this.Equals(p);
      }

      return false;
    }

    public bool Equals(Piece piece) {
      return this.Id == piece.Id;
    }
  }
}