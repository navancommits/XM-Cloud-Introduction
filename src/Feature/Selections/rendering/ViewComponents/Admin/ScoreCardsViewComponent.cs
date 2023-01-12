﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Mvp.Feature.Selections.Models.Admin;
using Mvp.Selections.Api.Model;
using Mvp.Selections.Client;
using Mvp.Selections.Client.Models;
using Mvp.Selections.Domain;
using Sitecore.AspNet.RenderingEngine.Binding;

namespace Mvp.Feature.Selections.ViewComponents.Admin
{
    [ViewComponent(Name = ViewComponentName)]
    public class ScoreCardsViewComponent : BaseViewComponent
    {
        public const string ViewComponentName = "AdminScoreCards";

        public ScoreCardsViewComponent(IViewModelBinder modelBinder, MvpSelectionsApiClient client)
            : base(modelBinder, client)
        {
        }

        public override async Task<IViewComponentResult> InvokeAsync()
        {
            IViewComponentResult result;
            ScoreCardsModel model = await ModelBinder.Bind<ScoreCardsModel>(ViewContext);
            if (model.IsEditing)
            {
                GenerateFakeDataForEdit(model);
                result = View(model);
            }
            else
            {
                await LoadSelections(model);
                await LoadMvpTypes(model);
                if (model.SelectedMvpTypeId > 0 && model.SelectedSelectionId != Guid.Empty)
                {
                    await LoadScoreCards(model);
                }

                result = string.IsNullOrWhiteSpace(model.ErrorMessage)
                    ? View(model)
                    : View("Error", model);
            }

            return result;
        }

        private static void GenerateFakeDataForEdit(ScoreCardsModel model)
        {
            Random rnd = new ();
            MvpType loremMvpType = new (1) { Name = "Lorem" };
            Country dolorCountry = new (1) { Name = "Dolor" };
            Selection conseceturSelection = new (Guid.NewGuid()) { Year = (short)(DateTime.Now.Year + rnd.Next(10, 20)) };

            model.SelectedSelectionId = conseceturSelection.Id;
            model.SelectedMvpTypeId = 1;
            model.MvpTypes.Add(loremMvpType);
            model.Selections.Add(conseceturSelection);

            model.ScoreCards.Add(new ScoreCard
            {
                Applicant = new Applicant
                {
                    MvpType = loremMvpType,
                    ApplicationId = Guid.NewGuid(),
                    Name = "Ipsum",
                    Country = dolorCountry
                },
                Average = rnd.Next(100),
                Median = rnd.Next(100),
                Max = rnd.Next(100),
                Min = rnd.Next(100),
                MaxReviewId = Guid.NewGuid(),
                MinReviewId = Guid.NewGuid(),
                ReviewCount = rnd.Next(10)
            });
            model.ScoreCards.Add(new ScoreCard
            {
                Applicant = new Applicant
                {
                    MvpType = loremMvpType,
                    ApplicationId = Guid.NewGuid(),
                    Name = "Amed",
                    Country = dolorCountry
                },
                Average = rnd.Next(100),
                Median = rnd.Next(100),
                Max = rnd.Next(100),
                Min = rnd.Next(100),
                MaxReviewId = Guid.NewGuid(),
                MinReviewId = Guid.NewGuid(),
                ReviewCount = rnd.Next(10)
            });
            model.ScoreCards.Add(new ScoreCard
            {
                Applicant = new Applicant
                {
                    MvpType = loremMvpType,
                    ApplicationId = Guid.NewGuid(),
                    Name = "Sid",
                    Country = dolorCountry
                },
                Average = rnd.Next(100),
                Median = rnd.Next(100),
                Max = rnd.Next(100),
                Min = rnd.Next(100),
                MaxReviewId = Guid.NewGuid(),
                MinReviewId = Guid.NewGuid(),
                ReviewCount = rnd.Next(10)
            });
        }

        private async Task LoadMvpTypes(ScoreCardsModel model)
        {
            Response<IList<MvpType>> mvpTypesResponse = await Client.GetMvpTypesAsync(1, short.MaxValue);
            if (mvpTypesResponse.StatusCode == HttpStatusCode.OK && mvpTypesResponse.Result != null)
            {
                model.MvpTypes.AddRange(mvpTypesResponse.Result);
            }
            else
            {
                model.ErrorMessage += mvpTypesResponse.Message;
            }
        }

        private async Task LoadSelections(ScoreCardsModel model)
        {
            Response<IList<Selection>> selectionsResponse = await Client.GetSelectionsAsync(1, short.MaxValue);
            if (selectionsResponse.StatusCode == HttpStatusCode.OK && selectionsResponse.Result != null)
            {
                model.Selections.AddRange(selectionsResponse.Result);
            }
            else
            {
                model.ErrorMessage += selectionsResponse.Message;
            }
        }

        private async Task LoadScoreCards(ScoreCardsModel model)
        {
            Response<IList<ScoreCard>> scoreCardsResponse = await Client.GetScoreCardsAsync(model.SelectedSelectionId, model.SelectedMvpTypeId);
            if (scoreCardsResponse.StatusCode == HttpStatusCode.OK && scoreCardsResponse.Result != null)
            {
                model.ScoreCards.AddRange(scoreCardsResponse.Result);
            }
            else
            {
                model.ErrorMessage += scoreCardsResponse.Message;
            }
        }
    }
}