{**************************************************************************************************}
{                                                                                                  }
{ Project JEDI Code Library (JCL)                                                                  }
{                                                                                                  }
{ The contents of this file are subject to the Mozilla Public License Version 1.1 (the "License"); }
{ you may not use this file except in compliance with the License. You may obtain a copy of the    }
{ License at http://www.mozilla.org/MPL/                                                           }
{                                                                                                  }
{ Software distributed under the License is distributed on an "AS IS" basis, WITHOUT WARRANTY OF   }
{ ANY KIND, either express or implied. See the License for the specific language governing rights  }
{ and limitations under the License.                                                               }
{                                                                                                  }
{ The Original Code is JclUsesDialog.pas.                                                          }
{                                                                                                  }
{ The Initial Developer of the Original Code is TOndrej (tondrej att t-online dott de).            }
{ Portions created by TOndrej are Copyright (C) of TOndrej.                                        }
{                                                                                                  }
{ Contributors:                                                                                    }
{                                                                                                  }
{**************************************************************************************************}
{                                                                                                  }
{ Last modified: $Date:: 2007-09-17 23:41:02 +0200 (lun. 17 sept. 2007)                          $ }
{ Revision:      $Rev:: 2175                                                                     $ }
{ Author:        $Author:: outchy                                                                $ }
{                                                                                                  }
{**************************************************************************************************}

unit JclUsesDialog;

interface

uses
  Windows, Messages, SysUtils, Classes, Graphics, Controls, Forms,
  Dialogs, StdCtrls, ComCtrls, ImgList;

type
  TFormUsesConfirm = class(TForm)
    ButtonCancel: TButton;
    ButtonOK: TButton;
    TreeImages: TImageList;
    TreeViewChanges: TTreeView;
    procedure ButtonOKClick(Sender: TObject);
    procedure TreeViewChangesKeyPress(Sender: TObject; var Key: Char);
    procedure TreeViewChangesMouseDown(Sender: TObject; Button: TMouseButton;
      Shift: TShiftState; X, Y: Integer);
  private
    FChangeList: TStrings;
    FErrors: TList;
    function ToggleNode(Node: TTreeNode): Boolean;
  public
    constructor Create(AOwner: TComponent; AChangeList: TStrings; Errors: TList); reintroduce;
  end;

implementation

uses
  CommCtrl,
  JclOtaResources, JclOtaUtils, JclUsesWizard;

{$R *.dfm}

constructor TFormUsesConfirm.Create(AOwner: TComponent; AChangeList: TStrings; Errors: TList);
const
  ActionStrings: array [TWizardAction] of string =
    (RsActionSkip, RsActionAdd, RsActionAdd, RsActionMove);
  SectionStrings: array [TWizardAction] of string =
    ('', RsSectionImpl, RsSectionIntf, RsSectionIntf);
var
  I, J: Integer;
  Node: TTreeNode;
begin
  inherited Create(AOwner);
  FChangeList := AChangeList;
  FErrors := Errors;
  for I := 0 to FChangeList.Count - 1 do
  begin
    Node := TreeViewChanges.Items.AddChildObject(nil, Format('%d. %s %s %s',
      [I + 1, ActionStrings[TWizardAction(FChangeList.Objects[I])], FChangeList[I],
      SectionStrings[TWizardAction(FChangeList.Objects[I])]]), Pointer(I));
    for J := 0 to FErrors.Count - 1 do
      with PErrorInfo(FErrors[J])^ do
        if AnsiCompareText(UsesName, FChangeList[I]) = 0 then
          with TreeViewChanges.Items.AddChild(Node, Format(RsUndeclIdent,
            [UnitName, LineNumber, Identifier, UsesName])) do
          begin
            ImageIndex := -1;
            SelectedIndex := -1;
          end;
    case TWizardAction(FChangeList.Objects[I]) of
      waSkip:
        Node.ImageIndex := 0;
      else
        Node.ImageIndex := 1;
    end;
    Node.SelectedIndex := Node.ImageIndex;

    Node.Expand(True);
  end;
  if FErrors.Count > 0 then
    with PErrorInfo(FErrors[0])^ do
      Caption := Format(RsConfirmChanges, [UnitName]);
end;

function TFormUsesConfirm.ToggleNode(Node: TTreeNode): Boolean;
begin
  if Node.ImageIndex = 0 then
  begin
    Node.ImageIndex := 1;
    Node.SelectedIndex := 1;
    Result := True;
  end
  else
  if Node.ImageIndex = 1 then
  begin
    Node.ImageIndex := 0;
    Node.SelectedIndex := 0;
    Result := True;
  end
  else
    Result := False;
end;

procedure TFormUsesConfirm.ButtonOKClick(Sender: TObject);
var
  Node: TTreeNode;
begin
  try
    with TreeViewChanges do
    begin
      Node := Items.GetFirstNode;
      while Assigned(Node) do
      begin
        if Node.ImageIndex = 0 then
          FChangeList.Objects[Integer(Node.Data)] := TObject(waSkip);
        Node := Node.GetNextSibling;
      end;
    end;
  except
    on ExceptionObj: TObject do
    begin
      JclExpertShowExceptionDialog(ExceptionObj);
      raise;
    end;
  end;
end;

procedure TFormUsesConfirm.TreeViewChangesKeyPress(Sender: TObject; var Key: Char);
var
  Node: TTreeNode;
begin
  try
    if Key = ' ' then
    begin
      Node := TreeViewChanges.Selected;
      if Assigned(Node) then
      begin
        if Node.Level > 0 then
          Node := Node.Parent;
        ToggleNode(Node);
        Key := #0;
      end;
    end;
  except
    on ExceptionObj: TObject do
    begin
      JclExpertShowExceptionDialog(ExceptionObj);
      raise;
    end;
  end;
end;

procedure TFormUsesConfirm.TreeViewChangesMouseDown(Sender: TObject;
  Button: TMouseButton; Shift: TShiftState; X, Y: Integer);
var
  Node: TTreeNode;
begin
  try
    with TreeViewChanges do
      if htOnIcon in GetHitTestInfoAt(X, Y) then
      begin
        Node := GetNodeAt(X, Y);
        if Assigned(Node) then
          ToggleNode(Node);
      end;
  except
    on ExceptionObj: TObject do
    begin
      JclExpertShowExceptionDialog(ExceptionObj);
      raise;
    end;
  end;
end;

end.
