//================================================================================
// COPNewForApp.h
//
// 役割:固有のアプリケーションに向けて、
//      クラスIDの入力を省略できるcopnewを提供する
//================================================================================

#ifndef __RTCOP_GENERATED_COPNEWFORAPP__
#define __RTCOP_GENERATED_COPNEWFORAPP__

// 元々のcopnewの読み込み
#include "RTCOP/COPNew.h"

// ベースクラスの宣言
namespace baselayer {
	class Hello;
}

namespace RTCOP {

namespace Generated {
// 知らないクラスのIDは-1
template<class Base>
inline const int GetBaseClassID() { return -1; }
// HelloのクラスID
template<>
inline const int GetBaseClassID<baselayer::Hello>() { return 0; }

} // namespace Generated {}

// レイヤードなクラスのインスタンス化
template<class Base, class... ArgTypes>
inline Core::LayerdObject<Base>* copnew(ArgTypes... args)
{
	// クラスIDの取得
	const int classId = Generated::GetBaseClassID<Base>();
	// 結果を返す
	return COPNew<Base>(classId, args...);
}

} // namespace RTCOP {}

#endif // !__RTCOP_GENERATED__COPNEW__
