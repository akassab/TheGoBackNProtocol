using A2Sender.enums;

namespace A2Sender.utils
{

    // TypeEnumUtils: A singleton class containing utilities for TypeEnum.
    public static class TypeEnumUtils
    {
        // Converts a uint to a TypeEnum type. Returns null when it can't.
        public static TypeEnum? UIntToTypeEnum(int typeInt) {
            if (Enum.IsDefined(typeof(TypeEnum), typeInt)) {
                return (TypeEnum) typeInt; 
            }
            else {
                return null;
            }
        }
    }
}
