using System;

namespace UIFramework.Resource
{
    #region ResKey
    public class ResKey : IEquatable<ResKey>
    {
        public bool UsePackage { get; private set; }
        public int ID { get; private set; }
        public string BundleName { get; private set; }
        public string BaseDir { get; private set; }
        public string AssetFileExt { get; private set; }
        public ResKey(bool usePackage, int id, string baseDir, string bundleName, string assetFileExt = "") 
        {
            UsePackage = usePackage;
            ID = id;
            BundleName = bundleName;
            BaseDir = baseDir;
            AssetFileExt = assetFileExt;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as ResKey);
        }

        public bool Equals(ResKey other)
        {
            // If parameter is null, return false.
            if (Object.ReferenceEquals(other, null))
            {
                return false;
            }

            // Optimization for a common success case.
            if (Object.ReferenceEquals(this, other))
            {
                return true;
            }

            // If run-time types are not exactly the same, return false.
            if (this.GetType() != other.GetType())
            {
                return false;
            }

            return this.ID == other.ID;
        }
        public static bool operator ==(ResKey lhs, ResKey rhs)
        {
            // Check for null on left side.
            if (Object.ReferenceEquals(lhs, null))
            {
                if (Object.ReferenceEquals(rhs, null))
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }
            // Equals handles case of null on right side.
            return lhs.Equals(rhs);
        }

        public static bool operator !=(ResKey lhs, ResKey rhs)
        {
            return !(lhs == rhs);
        }
    }
    #endregion ResKey

    public class ResourceKey
    {
        public static ResKey BASIC_WINDOW = new ResKey(true, 2, "BasicWindow/", "");
    }
}