//================================================================================
// LayerdObject.h
//
// 役割: レイヤードなオブジェクトを表すテンプレートクラス。
//       ユーザ定義のベースクラスを継承し、さらにレイヤードなオブジェクトに必要な要素が追加される。
//       RTCOPのインスタンス化では、ベースクラスを直接用いずに、必ずこのテンプレートクラスを用いること。
//================================================================================

#include "RTCOP/Core/LayerdObject.h"
#include "Core/LayerdObject_Private.h"
#include "RTCOP/Core/PartialClassMembers.h"
#include "DependentCode/ExecuteLayerdObjectFinalizer.h"

namespace RTCOP {
namespace Core {

//------------------------------------------------------
// プライベートクラスの実装
//------------------------------------------------------
// コンストラクタ
LayerdObject_Private::LayerdObject_Private()
{
	_LayerdObjectPtr = 0;
	_PartialClassMembers = 0;
	_NumOfLayers = -1;
}

// デストラクタ
LayerdObject_Private::~LayerdObject_Private()
{
	// レイヤ個数が負の数なら、仮想関数テーブル取得用に作られただけなので、ファイナライザを実行しない
	if (_NumOfLayers < 0)
	{
		return;
	}

	// パーシャルクラスの処分
	for (int i = 1; i < _NumOfLayers; ++i)
	{
		if (_PartialClassMembers[i] != 0)
		{
			// ファイナライザの実行
			volatile void* finalizer = _PartialClassMembers[i]->_Finalizer;
			if (finalizer != 0)
			{
				RTCOP::DependentCode::ExecuteLayerdObjectFinalizer(_LayerdObjectPtr, finalizer);
			}
			// delete
			delete _PartialClassMembers[i];
			_PartialClassMembers[i] = 0;
		}
	}
	delete[] _PartialClassMembers;
	_PartialClassMembers = 0;
	_LayerdObjectPtr = 0;
}

// レイヤードオブジェクトの共通の要素を初期化
void LayerdObject_Private::InitializeLayerdObject(void* obj, int numOfLayers)
{
	// LayerdObjectのポインタ
	_LayerdObjectPtr = obj;
	// レイヤの個数
	_NumOfLayers = numOfLayers;
	// レイヤ個数が負の数なら、仮想関数テーブル取得用に作られただけなので、中断する
	if (_NumOfLayers < 0)
	{
		return;
	}
	// 各パーシャルクラスのメンバ変数群
	_PartialClassMembers = new PartialClassMembers*[_NumOfLayers] {0};
}

//------------------------------------------------------
// LayerdObjectの実装
//------------------------------------------------------
// プライベートクラスの生成
LayerdObject_Private* LayerdObject_CreatePrivate()
{
	return new LayerdObject_Private();
}

// プライベートクラスの破棄
void LayerdObject_DestroyPrivate(LayerdObject_Private* privateClass)
{
	delete privateClass;
}

} // namespace Core {}
} // namespace RTCOP {}
