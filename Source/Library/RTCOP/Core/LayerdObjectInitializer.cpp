//================================================================================
// LayerdObjectInitializer.cpp
//
// 役割: 与えられたレイヤードなオブジェクトの初期化を行うクラス。
//       このクラスでは、レイヤードオブジェクトの仮想関数テーブルの書き換えなどを行う
//================================================================================

#include <cstdint>
#include "RTCOP/Core/LayerdObjectInitializer.h"
#include "Core/LayerdObjectInitializer_Private.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "RTCOP/Core/Layer.h"
#include "Core/LayerdObject_Private.h"
#include "Core/HasVFTableObject.h"

namespace RTCOP {
namespace Core {

//------------------------------------------------------
// プライベートクラスの実装
//------------------------------------------------------
// コンストラクタ
LayerdObjectInitializer_Private::LayerdObjectInitializer_Private(RTCOPManager* manager)
{
	_Manager = manager;
}

// デストラクタ
LayerdObjectInitializer_Private::~LayerdObjectInitializer_Private()
{
	_Manager = 0;
}

//------------------------------------------------------
// LayerdObjectInitializerの実装
//------------------------------------------------------
// コンストラクタ
LayerdObjectInitializer::LayerdObjectInitializer(RTCOPManager* manager)
{
	_Private = new LayerdObjectInitializer_Private(manager);
}

// デストラクタ
LayerdObjectInitializer::~LayerdObjectInitializer()
{
	delete _Private;
	_Private = 0;
}

// レイヤードオブジェクトの初期化
void* LayerdObjectInitializer::InitializeLayerdObject(void* obj, int classid, int size)
{
	// レイヤードオブジェクトの共通要素にアクセスするためのキャスト
	std::uintptr_t address = reinterpret_cast<std::uintptr_t>(obj) + size;
	std::uintptr_t* ptr = reinterpret_cast<std::uintptr_t*>(address);
	LayerdObject_Private* common = reinterpret_cast<LayerdObject_Private*>(*ptr);

	// 仮想関数テーブルの参照先を変更
	volatile void** vftable = _Private->_Manager->GetVirtualFunctionTable(classid);
	HasVFTableObject* vftobj = reinterpret_cast<HasVFTableObject*>(obj);
	vftobj->vptr = vftable;

	// レイヤ数から、レイヤードオブジェクトの初期化
	int numOfLayers = _Private->_Manager->GetNumOfLayers();
	common->InitializeLayerdObject(obj, numOfLayers);
	// レイヤごとのレイヤードオブジェクト初期化処理
	for (int i = 0; i < numOfLayers; ++i)
	{
		Layer* layer = _Private->_Manager->GetLayer(i);
		layer->InitializeLayerdObject(obj, classid);
	}

	// 結果を返す
	return obj;
}

} // namespace Core {}
} // namespace RTCOP {}
