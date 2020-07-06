//================================================================================
// PartialClassMembers.h
//
// 役割: パーシャルクラスのメンバ変数群。
//       RTCOPで自動生成されたパーシャルクラスで、そのメンバ変数を扱う際に、
//       このクラスを継承して、メンバ変数を追加する。
//================================================================================

#ifndef __RTCOP_CORE_PARTIALCLASSMEMBERS__
#define __RTCOP_CORE_PARTIALCLASSMEMBERS__

namespace RTCOP {
namespace Core {

class Layer;

// パーシャルクラスのメンバ変数群
class PartialClassMembers
{
public:
	// パーシャルクラスが属するレイヤ
	Layer* _Layer;
	// Proceed実現のための仮想関数テーブル
	volatile void** _VirtualFunctionTableForProceeding;
	// パーシャルクラスの処分を行うメソッドへの関数ポインタ
	volatile void* _Finalizer;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_PARTIALCLASSMEMBERS__
