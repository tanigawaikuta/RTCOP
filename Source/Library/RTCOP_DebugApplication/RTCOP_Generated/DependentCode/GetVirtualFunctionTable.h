//================================================================================
// GetVirtualFunctionTable.h
//
// 役割: LayerdObjectの仮想関数テーブルを取得するための関数を提供する。
//       この関数の実装は、プラットフォームによって異なる。
//================================================================================

#ifndef __RTCOP_GENERATED_DEPENDENTCODE_GETVIRTUALFUNCTIONTABLE__
#define __RTCOP_GENERATED_DEPENDENTCODE_GETVIRTUALFUNCTIONTABLE__

namespace RTCOP {
namespace Generated {

class BaseLayer;
class JapaneseLayer;
class EnglishLayer;

namespace DependentCode {

// Helloクラス用
namespace HelloClass {
// 仮想関数テーブルの取得
volatile void** GetVirtualFunctionTable(BaseLayer* layer);
volatile void** GetVirtualFunctionTable(JapaneseLayer* layer);
volatile void** GetVirtualFunctionTable(EnglishLayer* layer);

} // namespace HelloClass {}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}

#endif // !__RTCOP_GENERATED_DEPENDENTCODE_GETVIRTUALFUNCTIONTABLE__
