//================================================================================
// HasVFTableObject.h
//
// 役割: 仮想関数テーブル取得用クラス。
//       仮想関数テーブルが欲しいオブジェクトをこのクラスでreinterpret_castすることで、
//       取り出せるようになる。
//================================================================================

#ifndef __RTCOP_CORE_HASVTABLEOBJECT__
#define __RTCOP_CORE_HASVTABLEOBJECT__

namespace RTCOP {
namespace Core {

// 仮想関数テーブル取得用
class HasVFTableObject
{
public:
	// 仮想関数テーブル
	volatile void** vptr;
};

} // namespace Core {}
} // namespace RTCOP {}

#endif // !__RTCOP_CORE_HASVTABLEOBJECT__
