//================================================================================
// RTCOPManager.cpp
//
// 役割: RTCOPアプリケーション全体を管理するためのクラス。
//       このクラスでは、レイヤ、仮想関数テーブルなどを管理し、
//       様々なRTCOP機能実現オブジェクトから参照される。
//================================================================================

#include "RTCOP/Core/RTCOPManager.h"
#include "Core/RTCOPManager_Private.h"
#include "RTCOP/Core/Layer.h"

namespace RTCOP {
namespace Core {

//------------------------------------------------------
// プライベートクラスの実装
//------------------------------------------------------
RTCOPManager_Private::RTCOPManager_Private(int numOfLayers, int numOfBaseClasses, int* numOfBaseMethods)
{
	// レイヤ、ベースクラス、ベースメソッドの個数
	_NumOfLayers = numOfLayers;
	_NumOfBaseClasses = numOfBaseClasses;
	_NumOfBaseMethods = numOfBaseMethods;

	// 登録レイヤ、仮想関数テーブルのための配列を作成
	_RegisterdLayers = new Layer*[_NumOfLayers] { 0 };
	_VirtualFunctionTables = new volatile void**[_NumOfBaseClasses];
	for (int i = 0; i < _NumOfBaseClasses; ++i)
	{
		_VirtualFunctionTables[i] = new volatile void*[_NumOfBaseMethods[i]]{ 0 };
	}
}

// デストラクタ
RTCOPManager_Private::~RTCOPManager_Private()
{
	// 配列要素の処分
	for (int i = 0; i < _NumOfLayers; ++i)
	{
		// レイヤの処分
		delete _RegisterdLayers[i];
		_RegisterdLayers[i] = 0;
	}
	// 仮想関数テーブルの処分
	for (int i = 0; i < _NumOfBaseClasses; ++i)
	{
		delete[] _VirtualFunctionTables[i];
		_VirtualFunctionTables[i] = 0;
	}
	// 配列の処分
	delete[] _RegisterdLayers;
	_RegisterdLayers = 0;
	delete[] _VirtualFunctionTables;
	_VirtualFunctionTables = 0;
	delete[] _NumOfBaseMethods;
	_NumOfBaseMethods = 0;
}

//------------------------------------------------------
// RTCOPManagerの実装
//------------------------------------------------------
// コンストラクタ
RTCOPManager::RTCOPManager(int numOfLayers, int numOfBaseClasses, int* numOfBaseMethods)
{
	_Private = new RTCOPManager_Private(numOfLayers, numOfBaseClasses, numOfBaseMethods);
}

// デストラクタ
RTCOPManager::~RTCOPManager()
{
	delete _Private;
	_Private = 0;
}

// レイヤの登録
void RTCOPManager::RegisterLayer(Layer* layer)
{
	// レイヤの登録
	int layerID = layer->GetID();
	if (_Private->_RegisterdLayers[layerID] == 0)
	{
		_Private->_RegisterdLayers[layerID] = layer;
	}
}

// レイヤの取得
Layer* const RTCOPManager::GetLayer(int layerid) const
{
	return _Private->_RegisterdLayers[layerid];
}

// クラスIDで指定した仮想関数テーブルを取得
volatile void** const RTCOPManager::GetVirtualFunctionTable(int classid) const
{
	return _Private->_VirtualFunctionTables[classid];
}

// レイヤ、ベースクラス、ベースメソッドの個数を取得
const int RTCOPManager::GetNumOfLayers() const
{
	return _Private->_NumOfLayers;
}

// レイヤ、ベースクラス、ベースメソッドの個数を取得
const int RTCOPManager::GetNumOfBaseClasses() const
{
	return _Private->_NumOfBaseClasses;
}

// レイヤ、ベースクラス、ベースメソッドの個数を取得
int* const RTCOPManager::GetNumOfBaseMethods() const
{
	return _Private->_NumOfBaseMethods;
}

// レイヤ、ベースクラス、ベースメソッドの個数を取得
const int RTCOPManager::GetNumOfBaseMethods(int classid) const
{
	return _Private->_NumOfBaseMethods[classid];
}

} // namespace Core {}
} // namespace RTCOP {}
