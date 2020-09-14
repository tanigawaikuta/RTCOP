//================================================================================
// GetVirtualFunctionTable.cpp
//
// 役割: LayerdObjectの仮想関数テーブルを取得するための関数を提供する。
//       Windows x64のMinGW用の実装。
//================================================================================

#include "RTCOP_Generated/DependentCode/GetVirtualFunctionTable.h"
#include "RTCOP/Core/LayerdObject.h"
#include "RTCOP_Generated/BaseLayer.h"
#include "RTCOP_Generated/JapaneseLayer.h"
#include "RTCOP_Generated/EnglishLayer.h"

#include "Hello.h"

#ifdef WIN64_MINGW

// 仮想関数テーブル
asm volatile(".rtcop_vft1: .quad _ZTVN5RTCOP4Core12LayerdObjectIN9baselayer5HelloEEE+16");
asm volatile(".rtcop_vft2: .quad _ZTVN5RTCOP9Generated19JapaneseLayer_HelloE+16");
asm volatile(".rtcop_vft3: .quad _ZTVN5RTCOP9Generated18EnglishLayer_HelloE+16");

namespace RTCOP {
namespace Generated {
namespace DependentCode {

// Helloクラス用
namespace HelloClass {
// BaseLayer.Helloの仮想関数テーブルを取得
volatile void** GetVirtualFunctionTable(BaseLayer* layer)
{
	// 仮想関数テーブルの取得
	// 元のHelloは_ZTV5Hello
	volatile void** vft = 0;
	asm volatile("movq .rtcop_vft1(%%rip), %0" : "=r"(vft) : : "memory");

	return vft;
}

// JapaneseLayer.Helloの仮想関数テーブルを取得
volatile void** GetVirtualFunctionTable(JapaneseLayer* layer)
{
	// 仮想関数テーブルの取得
	volatile void** vft = 0;
	asm volatile("movq .rtcop_vft2(%%rip), %0" : "=r"(vft) : : "memory");

	return vft;
}

// EnglishLayer.Helloの仮想関数テーブルを取得
volatile void** GetVirtualFunctionTable(EnglishLayer* layer)
{
	// 仮想関数テーブルの取得
	volatile void** vft = 0;
	asm volatile("movq .rtcop_vft3(%%rip), %0" : "=r"(vft) : : "memory");

	return vft;
}

} // namespace HelloClass {}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}

#endif // WIN64_MINGW
