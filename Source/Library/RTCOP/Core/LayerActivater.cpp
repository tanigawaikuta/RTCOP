//================================================================================
// LayerActivater.cpp
//
// 役割: レイヤのアクティベーション、ディアクティベーションを行うクラス。
//================================================================================

#include "RTCOP/Core/LayerActivater.h"
#include "Core/LayerActivater_Private.h"
#include "RTCOP/Core/RTCOPManager.h"
#include "RTCOP/Core/Layer.h"

namespace RTCOP {
namespace Core {

//------------------------------------------------------
// プライベートクラスの実装
//------------------------------------------------------
// コンストラクタ
LayerActivater_Private::LayerActivater_Private(RTCOPManager* manager)
	: _CurrentTopLayer(0)
{
	_Manager = manager;
}

// デストラクタ
LayerActivater_Private::~LayerActivater_Private()
{
	_Manager = 0;
	_CurrentTopLayer = 0;
}

//------------------------------------------------------
// LayerActivaterの実装
//------------------------------------------------------
// コンストラクタ
LayerActivater::LayerActivater(RTCOPManager* manager)
{
	_Private = new LayerActivater_Private(manager);
}

// デストラクタ
LayerActivater::~LayerActivater()
{
	delete _Private;
	_Private = 0;
}

// レイヤアクティベート
void LayerActivater::ActivateLayer(int layerid)
{
	// レイヤ情報を取得
	Layer* activatingLayer = _Private->_Manager->GetLayer(layerid);
	// レイヤがすでにアクティブの場合
	if (activatingLayer->GetLayerState() == LayerState::Active)
	{
		// error
		return;
	}
	// アクティベート開始イベント
	activatingLayer->_RTCOP_OnActivating();

	// 仮想関数テーブルの書き換え
	int numOfBaseClasses = _Private->_Manager->GetNumOfBaseClasses();
	for (int i = 0; i < numOfBaseClasses; ++i)
	{
		// 各種、仮想関数テーブルを取得
		volatile void** vtable = _Private->_Manager->GetVirtualFunctionTable(i);
		volatile void** layer_vtable = activatingLayer->GetVirtualFunctionTable(i);
		volatile void** layer_proceed_vtable = activatingLayer->GetVirtualFunctionTableForProceeding(i);
		// メソッドごとに、アドレスを書き換える
		int numOfBaseMethods = _Private->_Manager->GetNumOfBaseMethods(i);
		for (int j = 0; j < numOfBaseMethods; ++j)
		{
			// パーシャルメソッドがあれば書き換え
			if ((layer_vtable[j] != 0) &&						// nullではない
				(layer_vtable[j] != vtable[j]))					// 先頭のものと違う内容である
			{
				// 仮想関数のアドレス書き換え
				layer_proceed_vtable[j] = vtable[j];
				vtable[j] = layer_vtable[j];
			}
		}
	}
	// レイヤの連結情報を更新
	if (_Private->_CurrentTopLayer != 0)
	{
		_Private->_CurrentTopLayer->SetLinkedPrevLayer(activatingLayer);
	}
	activatingLayer->SetLinkedNextLayer(_Private->_CurrentTopLayer);
	_Private->_CurrentTopLayer = activatingLayer;
	// レイヤ状態の変更
	activatingLayer->SetLayerState(LayerState::Active);
	// アクティベート終了イベント
	activatingLayer->_RTCOP_OnActivated();
}

// レイヤディアクティベート
void LayerActivater::DeactivateLayer(int layerid)
{
	// レイヤ情報を取得
	Layer* deactivatingLayer = _Private->_Manager->GetLayer(layerid);
	Layer* nextLayer = deactivatingLayer->GetLinkedNextLayer();
	Layer* prevLayer = deactivatingLayer->GetLinkedPrevLayer();
	// ディアクティベートレイヤがベースレイヤの場合、非アクティブの場合
	if (deactivatingLayer == _Private->_Manager->GetLayer(0) ||
		deactivatingLayer->GetLayerState() == LayerState::Inactive)
	{
		// error
		return;
	}
	// ディアクティベート開始イベント
	deactivatingLayer->_RTCOP_OnDeactivating();

	// 仮想関数テーブルの書き換え
	int numOfBaseClasses = _Private->_Manager->GetNumOfBaseClasses();
	for (int i = 0; i < numOfBaseClasses; ++i)
	{
		// 各種、仮想関数テーブルを取得
		volatile void** vtable = _Private->_Manager->GetVirtualFunctionTable(i);
		volatile void** layer_vtable = deactivatingLayer->GetVirtualFunctionTable(i);
		volatile void** layer_proceed_vtable = deactivatingLayer->GetVirtualFunctionTableForProceeding(i);
		// メソッドごとに、アドレスを書き換える
		int numOfBaseMethods = _Private->_Manager->GetNumOfBaseMethods(i);
		for (int j = 0; j < numOfBaseMethods; ++j)
		{
			// nullである
			if (layer_vtable[j] == 0)
			{
				// 次に移る
				continue;
			}
			// 先頭のメソッドと一致したら
			else if (layer_vtable[j] == vtable[j])
			{
				// 仮想関数のアドレス書き換え
				vtable[j] = layer_proceed_vtable[j];
				layer_proceed_vtable[j] = 0;
			}
			// ここに来るのは、間にあるメソッド
			else
			{
				// 前の関数が定義されているレイヤまで辿っていく
				Layer* lp = prevLayer;
				while (lp != 0)
				{
					volatile void** lpvt = lp->GetVirtualFunctionTableForProceeding(i);
					if (lpvt[j] == layer_vtable[j])
					{
						// 仮想関数のアドレス書き換え
						lpvt[j] = layer_proceed_vtable[j];
						break;
					}
					lp = lp->GetLinkedPrevLayer();
				}
			}
		}
	}
	// レイヤの連結情報を更新
	nextLayer->SetLinkedPrevLayer(prevLayer);
	if (prevLayer != 0)
	{
		prevLayer->SetLinkedNextLayer(nextLayer);
	}
	else
	{
		_Private->_CurrentTopLayer = nextLayer;
	}
	// レイヤ状態の変更
	deactivatingLayer->SetLayerState(LayerState::Inactive);
	// ディアクティベート終了イベント
	deactivatingLayer->_RTCOP_OnDeactivated();
}

} // namespace Core {}
} // namespace RTCOP {}
