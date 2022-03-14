using A2Sender.enums;

namespace A2Sender.utils
{
    // Packet data class
    public static class TypeEnumUtils
    {
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
