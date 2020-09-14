//================================================================================
// Layer.cpp
//
// 役割: レイヤごとの情報を扱うクラス。
//       パーシャルクラスのための仮想関数テーブルや、
//       Proceedを実現するための仮想関数テーブルを持っている。
//================================================================================

#include "RTCOP/Core/Layer.h"
#include "Core/Layer_Private.h"

namespace RTCOP {
namespace Core {

//------------------------------------------------------
// プライベートクラスの実装
//------------------------------------------------------
// コンストラクタ
Layer_Private::Layer_Private(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
	: _ID(id), _Name(name), _NumOfBaseClasses(numOfBaseClasses), _LinkedPrevLayer(0), _LinkedNextLayer(0), _LayerState(LayerState::Inactive)
{
	// レイヤ用の仮想関数テーブル群の配列作成
	volatile void*** vftables = new volatile void**[numOfBaseClasses];
	volatile void*** vftablesForProceeding = new volatile void**[numOfBaseClasses];
	for (int i = 0; i < numOfBaseClasses; ++i)
	{
		int num = numOfBaseMethods[i];
		vftables[i] = new volatile void*[num] { 0 };
		vftablesForProceeding[i] = new volatile void*[num] { 0 };
	}
	// メンバ変数に設定
	_VirtualFunctionTables = vftables;
	_VirtualFunctionTablesForProceeding = vftablesForProceeding;
}

// デストラクタ
Layer_Private::~Layer_Private()
{
	if (_VirtualFunctionTables != 0)
	{
		for (int i = 0; i < _NumOfBaseClasses; ++i)
		{
			delete[] _VirtualFunctionTables[i];
			_VirtualFunctionTables[i] = 0;
		}
		delete[] _VirtualFunctionTables;
		_VirtualFunctionTables = 0;
	}
	if (_VirtualFunctionTablesForProceeding != 0)
	{
		for (int i = 0; i < _NumOfBaseClasses; ++i)
		{
			delete[] _VirtualFunctionTablesForProceeding[i];
			_VirtualFunctionTablesForProceeding[i] = 0;
		}
		delete[] _VirtualFunctionTablesForProceeding;
		_VirtualFunctionTablesForProceeding = 0;
	}
	_LinkedPrevLayer = 0;
	_LinkedNextLayer = 0;
}

//------------------------------------------------------
// Layerの実装
//------------------------------------------------------
// コンストラクタ
Layer::Layer(const int id, const char* const name, int numOfBaseClasses, int* numOfBaseMethods)
{
	_Private = new Layer_Private(id, name, numOfBaseClasses, numOfBaseMethods);
}

// デストラクタ
Layer::~Layer()
{
	delete _Private;
	_Private = 0;
}

// IDの取得
const int Layer::GetID() const
{
	return _Private->_ID;
}

// 名前の取得
const char* const Layer::GetName() const
{
	return _Private->_Name;
}

// パーシャルクラスのための仮想関数テーブルの取得
volatile void** const Layer::GetVirtualFunctionTable(int classid) const
{
	return _Private->_VirtualFunctionTables[classid];
}

// Proceedを実現するための仮想関数テーブルの取得
volatile void** const Layer::GetVirtualFunctionTableForProceeding(int classid) const
{
	return _Private->_VirtualFunctionTablesForProceeding[classid];
}

// レイヤ状態の取得
LayerState Layer::GetLayerState() const
{
	return _Private->_LayerState;
}

// レイヤ状態の変更
void Layer::SetLayerState(LayerState state)
{
	_Private->_LayerState = state;
}

// 前に連結されているアクティブなレイヤのゲッタ
Layer* const Layer::GetLinkedPrevLayer() const
{
	return _Private->_LinkedPrevLayer;
}

// 後に連結されているアクティブなレイヤのゲッタ
Layer* const Layer::GetLinkedNextLayer() const
{
	return _Private->_LinkedNextLayer;
}

// 前に連結されているアクティブなレイヤのセッタ
void Layer::SetLinkedPrevLayer(Layer* const layer)
{
	_Private->_LinkedPrevLayer = layer;
}

// 後に連結されているアクティブなレイヤのセッタ
void Layer::SetLinkedNextLayer(Layer* const layer)
{
	_Private->_LinkedNextLayer = layer;
}

//------------------------------------------------------
// イベント発生時に実行されるメソッド
//------------------------------------------------------
// アクティベート開始時に実行される
void Layer::_RTCOP_OnActivating()
{
}

// アクティベート終了時に実行される
void Layer::_RTCOP_OnActivated()
{
}

// ディアクティベート開始時に実行される
void Layer::_RTCOP_OnDeactivating()
{
}

// ディアクティベート終了時に実行される
void Layer::_RTCOP_OnDeactivated()
{
}

} // namespace Core {}
} // namespace RTCOP {}
