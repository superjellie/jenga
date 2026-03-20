
[System.Serializable]
public struct MaybeNull<T> {

    public bool isNotNull;
    public T item;

    public bool isNull { get => !isNotNull; set => isNotNull = !value; }

    MaybeNull(T item, bool isNull) : this() {
        this.isNull = isNull;
        this.item = item;
    }
      
    public MaybeNull(T item) : this(item, item == null) { }
    public static MaybeNull<T> Null() { return new MaybeNull<T>(); }

    public static implicit operator T(MaybeNull<T> nullObject)
        => nullObject.item;
    public static implicit operator MaybeNull<T>(T item)
        => new(item);

    public override string ToString() 
        => (item != null) ? item.ToString() : "NULL";

    public override bool Equals(object obj) {
        if (obj == null)
            return this.isNull;

        if (!(obj is MaybeNull<T> no))
            return false;

        if (this.isNull)
            return no.isNull;

        if (no.isNull)
            return false;

        return this.item.Equals(no.item);
    }

    public override int GetHashCode() {
        if (isNull)
            return 0;

        var result = item.GetHashCode();

        if (result >= 0)
            result++;

        return result;
    }

    public static bool operator==(MaybeNull<T> x, MaybeNull<T> y)
        => x.Equals(y);
    public static bool operator!=(MaybeNull<T> x, MaybeNull<T> y)
        => !x.Equals(y);
}