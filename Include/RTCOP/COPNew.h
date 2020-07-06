//================================================================================
// COPNew.h
//
// 役割: ユーザにcopnewを提供する。
//       copnewはレイヤードなクラスをインスタンス化する機能である。
//       これによってインスタンス化されたオブジェクトは、
//       アクティブなレイヤの変更時に振る舞いが変わる
//================================================================================

#ifndef __RTCOP_COPNEW__
#define __RTCOP_COPNEW__

// テンプレートメソッドcopnew実現のために必要なヘッダの読み込み
#include "RTCOP/Framework.h"
#include "RTCOP/Core/LayerdObject.h"
#include "RTCOP/Core/LayerdObjectInitializer.h"

namespace RTCOP {

// レイヤードなクラスのインスタンス化
template<class Base, class... ArgTypes>
Core::LayerdObject<Base>* COPNew(int classid, ArgTypes... args)
{
	// クラスIDが不正の場合nullを返す
	if (classid < 0) return 0;

	// レイヤードオブジェクトのインスタンス化
	Core::LayerdObject<Base>* obj = new Core::LayerdObject<Base>(args...);
	// ベースクラスのサイズ取得
	int size = obj->GetBaseClassSize();
	// レイヤードオブジェクトの初期化
	Framework::Instance->GetLayerdObjectInitializer()->InitializeLayerdObject(obj, classid, size);

	// 結果を返す
	return obj;
}

} // namespace RTCOP {}

#endif // !__RTCOP_COPNEW__
