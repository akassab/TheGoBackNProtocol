using A2Sender.enums;

namespace A2Sender.utils
{

    // TypeEnumUtils: A singleton class containing utilities for TypeEnum.
    public static class TypeEnumUtils
    {
        // UIntToTypeEnum(typeUint): Converts a uint to a TypeEnum type. Returns null when it can't.
        public static TypeEnum? UIntToTypeEnum(uint typeUint) {
            if (Enum.IsDefined(typeof(TypeEnum), typeUint)) {
                return (TypeEnum) typeUint; 
            }
            else {
                return null;
            }
        }
    }
}
