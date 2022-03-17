using A2Receiver.enums;

namespace A2Receiver.utils
{
    // TypeEnumUtils: A singleton class containing utilities for TypeEnum.
    public static class TypeEnumUtils
    {
        // Converts a uint to a TypeEnum type. Returns null when it can't.
        public static TypeEnum? UIntToTypeEnum(int typeUint) {
            if (Enum.IsDefined(typeof(TypeEnum), typeUint)) {
                return (TypeEnum) typeUint; 
            }
            else {
                return null;
            }
        }
    }
}
