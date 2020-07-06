//================================================================================
// GetVirtualFunctionTable.cpp
//
// 役割: LayerdObjectの仮想関数テーブルを取得するための関数を提供する。
//       Linux arm64の実装。
//================================================================================

#include "RTCOP_Generated/DependentCode/GetVirtualFunctionTable.h"
#include "RTCOP/Core/LayerdObject.h"
#include "RTCOP_Generated/BaseLayer.h"
#include "RTCOP_Generated/JapaneseLayer.h"
#include "RTCOP_Generated/EnglishLayer.h"

#include "Hello.h"

#ifdef LINUX_ARM64

// 仮想関数テーブル
asm volatile(".rtcop_vft1: .xword _ZTVN5RTCOP4Core12LayerdObjectI5HelloEE+16");
asm volatile(".rtcop_vft2: .xword _ZTVN5RTCOP9Generated19JapaneseLayer_HelloE+16");
asm volatile(".rtcop_vft3: .xword _ZTVN5RTCOP9Generated18EnglishLayer_HelloE+16");

namespace RTCOP {
namespace Generated {
namespace DependentCode {

// Helloクラス用
namespace HelloClass {
// BaseLayer.Helloの仮想関数テーブルを取得
volatile void** GetVirtualFunctionTable(BaseLayer* layer)
{
	// 仮想関数テーブルの取得
	volatile void** vft = (volatile void**)"A";		// 文字列リテラルが一つはないと、何故か動かない…
	asm volatile("ldr %0, .rtcop_vft1" : "=r"(vft) : : "memory");

	return vft;
}

// JapaneseLayer.Helloの仮想関数テーブルを取得
volatile void** GetVirtualFunctionTable(JapaneseLayer* layer)
{
	// 仮想関数テーブルの取得
	volatile void** vft = 0;
	asm volatile("ldr %0, .rtcop_vft2" : "=r"(vft) : : "memory");

	return vft;
}

// EnglishLayer.Helloの仮想関数テーブルを取得
volatile void** GetVirtualFunctionTable(EnglishLayer* layer)
{
	// 仮想関数テーブルの取得
	volatile void** vft = 0;
	asm volatile("ldr %0, .rtcop_vft3" : "=r"(vft) : : "memory");

	return vft;
}

} // namespace HelloClass {}

} // namespace DependentCode {}
} // namespace Generated {}
} // namespace RTCOP {}

#endif // LINUX_ARM64
